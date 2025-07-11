using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Instancium.TagKit.Core.Middleware
{
    public class TagKitHostMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TagKitHostMiddleware> _logger;

        public TagKitHostMiddleware(RequestDelegate next, ILogger<TagKitHostMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            await _next(context);


            if (context.Items.TryGetValue("__inst_render_mode", out var mode) &&
                mode?.ToString() == "fragment")
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBody);
                return;
            }

            if (!IsHtmlResponse(context.Response))
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBody);
                return;
            }

            buffer.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(buffer);
            var html = await reader.ReadToEndAsync();

            var modifiedHtml = InjectResources(html);

            var outputBytes = Encoding.UTF8.GetBytes(modifiedHtml);
            context.Response.ContentLength = outputBytes.Length;
            context.Response.Body = originalBody;
            await context.Response.Body.WriteAsync(outputBytes, 0, outputBytes.Length);
        }

        private static bool IsHtmlResponse(HttpResponse response)
        {
            var contentType = response.ContentType;
            return !string.IsNullOrWhiteSpace(contentType) &&
                   contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase);
        }

        private static string InjectResources(string html)
        {
            if (html.Contains("/instancium/resources/inst.js"))
                return html;

            var headInjection = BuildHeadInjection();
            var bodyInjection = BuildFooterInjection();

            html = InjectBeforeTag(html, "</head>", headInjection);
            html = InjectBeforeTag(html, "</body>", bodyInjection);

            return html;
        }

        private static string InjectBeforeTag(string html, string tag, string content)
        {
            var index = html.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
            return index >= 0
                ? html.Insert(index, content + "\n")
                : html + "\n" + content;
        }

        private static string BuildHeadInjection()
        {
            var sb = new StringBuilder();
            foreach (var hash in ResourceManifest.Current.StyleHashes)
                sb.AppendLine($"<link href=\"/instancium/resources/style-{hash}.css\" rel=\"stylesheet\">");

            return sb.ToString();
        }

        private static string BuildFooterInjection()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!-- Instancium runtime -->");
            sb.AppendLine("<script src=\"/instancium/resources/inst.js?v=1\" defer></script>");

            foreach (var hash in ResourceManifest.Current.ScriptHashes)
                sb.AppendLine($"<script src=\"/instancium/resources/script-{hash}.js\" defer></script>");

            return sb.ToString();
        }
    }


}
