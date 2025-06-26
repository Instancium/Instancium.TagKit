using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Instancium.TagKit.Core.Utils
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
        public required string ComponentId {get;set;}

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

            html = ExtractInnerTagHelperContent(html);
            return html;
        }

        private async Task<string> EmbedCssAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            var pattern = $"<link href=\"./{baseName}.css\" rel=\"stylesheet\">";
            if (!html.Contains(pattern))
                return html;

            string css = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{baseName}.css");

            // [STEP 1] Wrap CSS with component ID for scoping
            css = ScopeCssToComponent(css);
            string styleTag = $"<style>\n{css}\n</style>";

            // [STEP 2] Remove external link
            html = html.Replace(pattern, string.Empty);

            // [STEP 3] Insert inline <style> right after <tag-helper>
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
        /// By default, wraps CSS selectors with the given component ID for style isolation.
        /// If the CSS contains the directive `/* @no-scope */`, it will be returned unmodified.
        /// </summary>
        /// <param name="css">Raw CSS content from the embedded resource.</param>
        /// <param name="componentId">The ID of the DOM element for scoping.</param>
        /// <returns>Scoped CSS unless opt-out directive is present.</returns>
        private string ScopeCssToComponent(string css)
        {
            // Skip scoping if opt-out directive is present
            if (css.Contains("/* @no-scope */"))
                return css;

            var lines = css.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var scoped = new StringBuilder();

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("/*"))
                {
                    scoped.AppendLine(line);
                    continue;
                }

                if (line.Contains('{'))
                    scoped.AppendLine($"#{ComponentId} {line}");
                else
                    scoped.AppendLine(line);
            }

            return scoped.ToString();
        }

        /// <summary>
        /// Inlines the JavaScript logic for the component by replacing a reference like
        /// <c>&lt;script src="./Component.js"&gt;&lt;/script&gt;</c> with an inline IIFE (Immediately Invoked Function Expression).
        /// 
        /// This allows all component logic to be embedded as part of the final markup,
        /// making it portable and testable without external dependencies. Lifecycle hooks are
        /// no longer automatically invoked — the component itself is responsible for exposing and triggering them.
        /// </summary>
        /// <param name="html">Raw component markup loaded from the embedded HTML resource.</param>
        /// <param name="baseName">Base name of the component (e.g., TestTagHelper).</param>
        /// <param name="baseNamespace">Root namespace where embedded resources are located.</param>
        /// <param name="assembly">Assembly containing the embedded JavaScript resource.</param>
        /// <returns>HTML with inline JavaScript logic injected in place of the external script reference.</returns>

        private async Task<string> EmbedJsAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            var pattern = $"<script src=\"./{baseName}.js\"></script>";

            if (html.Contains(pattern))
            {
                string js = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{baseName}.js");
                string scriptTag = $"<script>\n(() => {{\n{js}\n}})();\n</script>";

                html = html.Replace(pattern, string.Empty);

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
            using var stream = assembly.GetManifestResourceStream(resourcePath)
                ?? throw new FileNotFoundException($"Resource not found: {resourcePath}");

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
