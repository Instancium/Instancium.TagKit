using Instancium.TagKit.Core.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Instancium.TagKit.Core.Samples.HelloTagHelper;

/// <summary>
/// Demo tag helper that renders the current UTC time using an embedded HTML view.
/// </summary>
[HtmlTargetElement("inst-hello")]
public class HelloTagHelper : TagHelperBase
{
    public HelloTagHelper(
        IHttpContextAccessor httpContextAccessor,
        IOptions<AppSettings> options)
        : base(httpContextAccessor, options)
    { }

    /// <summary>
    /// Replaces the {{CurrentTime}} token in the embedded HTML template with the current time.
    /// </summary>
    protected override async Task<string> RenderHtmlAsync(TagHelperContext context, TagHelperOutput output)
    {
        string rawHtml = await ReadOwnHtmlAsync();
        rawHtml = rawHtml.Replace("{{CurrentTime}}", DateTime.UtcNow.ToString("u"));
        return rawHtml;
    }
}
