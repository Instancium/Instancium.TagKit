﻿using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Instancium.TagKit.Core.Rendering
{
    /// <summary>
    /// Provides methods for rendering embedded HTML views,
    /// including automatic inlining of associated CSS and JavaScript assets.
    /// </summary>
    public class TagViewBuilder
    {
        /// <summary>
        /// The unique ID used to scope DOM styles, hook API registration, and JS execution.
        /// This maps directly to the tag-helper's rendered HTML 'id' attribute.
        /// </summary>
        public required string ComponentId { get; set; }

        /// <summary>
        /// Determines whether CSS and JS resources should be linked as external URLs
        /// (served via the resource controller) instead of being inlined directly into the HTML output.
        /// 
        /// When set to <c>true</c>, component styles and scripts will be registered in the runtime
        /// registry and inserted as references (e.g., <link> / <script src>).
        /// When <c>false</c>, the assets will be inlined into the markup for self-contained rendering.
        /// </summary>
        public bool UseResourceLink { get; set; }

        public string LanguageCode { get; set; } = "en";

        /// <summary>
        /// Reads and composes a view from embedded HTML, CSS, and JS resources based on the tag helper type.
        /// </summary>
        /// <param name="tagHelperType">The type that owns the embedded view resources.</param>
        public async Task<string> ReadFromResourceAsync(Type tagHelperType)
        {
            return await ReadFromResourceAsync($"{tagHelperType.Name}.html", tagHelperType);
        }

        /// <summary>
        /// Reads the specified HTML file, inlines its related CSS and JS, and returns the final markup.
        /// </summary>
        /// <param name="fileName">The embedded HTML resource file name.</param>
        /// <param name="tagHelperType">The owner type to resolve the assembly and namespace context.</param>
        private async Task<string> ReadFromResourceAsync(string fileName, Type tagHelperType)
        {
            var assembly = tagHelperType.Assembly;
            var baseNamespace = tagHelperType.Namespace!;

            string html = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{fileName}");
            string baseName = Path.GetFileNameWithoutExtension(fileName);

            html = await EmbedCssAsync(html, baseName, baseNamespace, assembly);
            html = await EmbedJsAsync(html, baseName, baseNamespace, assembly);
            html = await EmbedI18nAsync(html, baseName, baseNamespace, assembly);
            html = ExtractInnerTagHelperContent(html);

            return html;
        }

        private async Task<string> EmbedCssAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            var pattern = $@"<link\s[^>]*?href\s*=\s*""\.\/{baseName}\.css""[^>]*?>";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!regex.IsMatch(html))
                return html;

            string css = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{baseName}.css");

            // === [ STEP 1 ] Scope component CSS using the element ID ===
            css = ScopeCssToComponent(css);

            // === [ STEP 2 ] Resource-link mode: register and defer insertion ===
            if (UseResourceLink)
            {
                var hash = ResourceHelpers.RegisterAndTrackStyle(css);
                ResourceManifest.Current.AddStyle(hash); // Style link will be rendered separately (e.g., in <head>)
                return html;
            }

            // === [ STEP 3 ] Inline style: inject <style> block into component ===
            string styleTag = $"<style>\n{css}\n</style>";
            var tagOpen = "<tag-helper";
            int tagStartIndex = html.IndexOf(tagOpen, StringComparison.OrdinalIgnoreCase);
            if (tagStartIndex >= 0)
            {
                int openTagEndIndex = html.IndexOf('>', tagStartIndex);
                if (openTagEndIndex >= 0)
                    return html.Insert(openTagEndIndex + 1, "\n" + styleTag);
            }

            return styleTag + "\n" + html;
        }

        /// <summary>
        /// Attempts to localize the given HTML by loading a JS-based translation dictionary
        /// from an embedded resource and replacing all @_Key_ markers with localized values.
        /// </summary>
        /// <param name="html">The original HTML content containing localization markers.</param>
        /// <param name="baseName">The base name of the component (e.g. "HelloTagHelper").</param>
        /// <param name="baseNamespace">The namespace where the embedded JS resource is located.</param>
        /// <param name="assembly">The assembly containing the embedded resource.</param>
        /// <returns>Localized HTML with all @_Key_ markers replaced, or the original HTML if localization fails.</returns>
        private async Task<string> EmbedI18nAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            // Construct the expected resource path for the JS-based dictionary
            var resourceFileName = $"{baseName}_i18n_{LanguageCode}.js";
            var resourcePath = $"{baseNamespace}.{resourceFileName}";

            // Attempt to read the embedded JS file (returns empty string if not found)
            string js = await ReadEmbeddedFileAsync(assembly, resourcePath);
            if (string.IsNullOrWhiteSpace(js))
                return html; // Fallback: return original HTML if resource is missing

            // Extract the JSON object from the JS file using regex
            var match = Regex.Match(js, @"=\s*(\{[\s\S]*\})\s*;", RegexOptions.Compiled);
            if (!match.Success)
                return html; // Fallback: return original HTML if JSON block not found

            var json = match.Groups[1].Value;
            Dictionary<string, string>? translations = null;

            try
            {
                // Attempt to deserialize the JSON into a dictionary
                translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch
            {
                return html; // Fallback: return original HTML if JSON is invalid
            }

            if (translations == null || translations.Count == 0)
                return html; // Fallback: no translations available

            // Replace all @_Key_ markers in the HTML with localized values
            var pattern = new Regex(@"@(_[A-Za-z0-9]+_)", RegexOptions.Compiled);
            var localized = pattern.Replace(html, match =>
            {
                var key = match.Groups[1].Value;
                return translations.TryGetValue(key, out var value)
                    ? value
                    : $"[MISSING {key}]"; // Optional: mark missing keys visibly
            });

            return localized;
        }



        /// <summary>
        /// By default, wraps CSS selectors with the given component ID for style isolation.
        /// If the CSS contains the directive `/* @no-scope */`, it will be returned unmodified.
        /// </summary>
        /// <param name="css">Raw CSS content from the embedded resource.</param>
        /// <param name="componentId">The ID of the DOM element for scoping.</param>
        /// <returns>Scoped CSS unless opt-out directive is present.</returns>
        private string ScopeCssToComponent(string css)
        {
            if (css.Contains("/* @no-scope */"))
                return css;

            var lines = css.Split('\n');
            var scoped = new StringBuilder();
            var insideAtBlock = false;
            var indentLevel = 0;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();

                // Track block depth to handle @media properly
                if (line.StartsWith("@media") || line.StartsWith("@supports"))
                {
                    insideAtBlock = true;
                    indentLevel++;
                    scoped.AppendLine(rawLine); // leave it untouched
                    continue;
                }

                if (line.Contains("{"))
                {
                    indentLevel++;
                }

                if (line.Contains("}"))
                {
                    indentLevel--;
                    if (indentLevel == 0)
                        insideAtBlock = false;
                }

                // Skip empty, comment, or non-selector lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("/*") || line.StartsWith("*") || line.StartsWith("@"))
                {
                    scoped.AppendLine(rawLine);
                    continue;
                }

                // If line likely contains a CSS selector
                if (line.Contains("{"))
                {
                    var index = rawLine.IndexOf('{');
                    var beforeBrace = rawLine.Substring(0, index).Trim();
                    var afterBrace = rawLine.Substring(index);

                    // Scope each selector in a group, e.g., `.a, .b` -> `#id .a, #id .b`
                    var scopedSelectors = string.Join(", ",
                        beforeBrace.Split(',')
                            .Select(sel => $"#{ComponentId} {sel.Trim()}"));

                    scoped.AppendLine($"{scopedSelectors}{afterBrace}");
                }
                else
                {
                    scoped.AppendLine(rawLine);
                }
            }

            return scoped.ToString();
        }



        /// <summary>
        /// Inlines the JavaScript logic for the component by locating a reference like
        /// <c>&lt;script src="./Component.js"&gt;&lt;/script&gt;</c> and embedding its contents
        /// as a scoped inline IIFE (Immediately Invoked Function Expression).
        ///
        /// The JavaScript file is expected to begin with:
        /// <c>var el = document.querySelector("tag-helper");</c>
        /// During rendering, this selector will be rewritten to reference the actual component ID:
        /// <c>document.querySelector("#tag-abc123")</c>, allowing all logic to remain scoped within the component root.
        ///
        /// This design enables strict DOM isolation, avoids global bindings, and promotes testability
        /// and portability of the component logic across render contexts (SSR, hydration, snapshot testing, etc.).
        /// </summary>
        /// <param name="html">Raw HTML markup loaded from the embedded .html resource.</param>
        /// <param name="baseName">Logical base name of the component (e.g., <c>HelloCard</c>).</param>
        /// <param name="baseNamespace">Namespace context in which the resource files reside.</param>
        /// <param name="assembly">Assembly where embedded .js and .html resources are defined.</param>
        /// <returns>Processed HTML with inline JavaScript embedded and scoped to the component element.</returns>

        private async Task<string> EmbedJsAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            // === [ STEP 1 ] Look for the external script pattern in embedded HTML ===
            var pattern = $"<script src=\"./{baseName}.js\"></script>";

            if (html.Contains(pattern))
            {
                // === [ STEP 2 ] Load and bind embedded JS to the component ===
                // We assume the JS begins with: var el = document.querySelector("tag-helper");
                // This placeholder selector will be rewritten to use the actual ComponentId
                // so the script remains scoped and testable across different render contexts.
                string js = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{baseName}.js");
                js = js.Replace("document.querySelector(\"tag-helper\")", $"document.querySelector(\"#{ComponentId}\")");


                // === [ STEP 3 ] Resource-link mode: register and defer inclusion ===
                if (UseResourceLink)
                {
                    var hash = ResourceHelpers.RegisterAndTrackScript(js);
                    ResourceManifest.Current.AddScript(hash); // Will be rendered as <script src="/...script-{hash}.js">
                    return html; // Do not inline — external reference will be added separately
                }

                // === [ STEP 4 ] Inline mode: embed <script> block inside the component ===
                string scriptTag = $"<script>\n(() => {{\n{js}\n}})();\n</script>";

                int insertIndex = html.IndexOf("</tag-helper>", StringComparison.OrdinalIgnoreCase);
                html = insertIndex >= 0
                    ? html.Insert(insertIndex, scriptTag + "\n")
                    : html + "\n" + scriptTag;
            }

            return html;
        }


        private string ExtractInnerTagHelperContent(string html)
        {
            var startTag = "<tag-helper";
            var endTag = "</tag-helper>";

            int startIndex = html.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1) return html;

            int tagEndIndex = html.IndexOf('>', startIndex);
            if (tagEndIndex == -1) return html;

            int endIndex = html.IndexOf(endTag, tagEndIndex, StringComparison.OrdinalIgnoreCase);
            if (endIndex == -1) return html;

            return html.Substring(tagEndIndex + 1, endIndex - tagEndIndex - 1).Trim();
        }

        private async Task<string> ReadEmbeddedFileAsync(Assembly assembly, string resourcePath)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream == null)
                    return string.Empty;

                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
