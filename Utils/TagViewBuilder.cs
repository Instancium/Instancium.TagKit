using System.Reflection;

namespace Instancium.TagKit.Core.Utils
{
    /// <summary>
    /// Provides methods for rendering embedded HTML views,
    /// including automatic inlining of associated CSS and JavaScript assets.
    /// </summary>
    public class TagViewBuilder
    {
        /// <summary>
        /// Reads and composes a view from embedded HTML, CSS, and JS resources based on the tag helper type.
        /// </summary>
        /// <param name="tagHelperType">The type that owns the embedded view resources.</param>
        public static async Task<string> ReadFromResourceAsync(Type tagHelperType)
        {
            return await ReadFromResourceAsync($"{tagHelperType.Name}.html", tagHelperType);
        }

        /// <summary>
        /// Reads the specified HTML file, inlines its related CSS and JS, and returns the final markup.
        /// </summary>
        /// <param name="fileName">The embedded HTML resource file name.</param>
        /// <param name="tagHelperType">The owner type to resolve the assembly and namespace context.</param>
        public static async Task<string> ReadFromResourceAsync(string fileName, Type tagHelperType)
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

        private static async Task<string> EmbedCssAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            var pattern = $"<link href=\"./{baseName}.css\" rel=\"stylesheet\">";
            if (html.Contains(pattern))
            {
                string css = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{baseName}.css");
                string styleTag = $"<style>\n{css}\n</style>";

                html = html.Replace(pattern, string.Empty);

                var tagOpen = "<tag-helper";
                int tagStartIndex = html.IndexOf(tagOpen, StringComparison.OrdinalIgnoreCase);
                if (tagStartIndex >= 0)
                {
                    int openTagEndIndex = html.IndexOf('>', tagStartIndex);
                    if (openTagEndIndex >= 0)
                        return html.Insert(openTagEndIndex + 1, "\n" + styleTag);
                }

                html = styleTag + "\n" + html;
            }

            return html;
        }

        private static async Task<string> EmbedJsAsync(string html, string baseName, string baseNamespace, Assembly assembly)
        {
            var pattern = $"<script src=\"./{baseName}.js\"></script>";
            if (html.Contains(pattern))
            {
                string js = await ReadEmbeddedFileAsync(assembly, $"{baseNamespace}.{baseName}.js");
                string scriptTag = $"<script>\n(() => {{\n{js}\n}})();\n</script>";

                html = html.Replace(pattern, string.Empty);

                int insertIndex = html.IndexOf("</tag-helper>", StringComparison.OrdinalIgnoreCase);
                if (insertIndex >= 0)
                    html = html.Insert(insertIndex, scriptTag + "\n");
                else
                    html += "\n" + scriptTag;
            }

            return html;
        }

        private static string ExtractInnerTagHelperContent(string html)
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

        private static async Task<string> ReadEmbeddedFileAsync(Assembly assembly, string resourcePath)
        {
            using var stream = assembly.GetManifestResourceStream(resourcePath)
                ?? throw new FileNotFoundException($"Resource not found: {resourcePath}");

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
