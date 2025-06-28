using Instancium.TagKit.Core.Config;
using Instancium.TagKit.Core.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Instancium.TagKit.Core.Components.TestTagHelper;

/// <summary>
/// Demo tag helper that renders the current UTC time using an embedded HTML view.
/// </summary>
[HtmlTargetElement("test-tag")]
public class TestTagHelper : TagHelperBase
{
    public TestTagHelper(
        IHttpContextAccessor httpContextAccessor,
        IOptions<AppSettings> options,
        IStringLocalizerFactory localizerFactory)
        : base(httpContextAccessor, options, localizerFactory)
    {}

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
