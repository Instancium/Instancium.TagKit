using Instancium.TagKit.Core;
using Instancium.TagKit.Core.Config;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Instancium.TagKit.DemoApp.Components.Counter
{
    public class CounterTagHelper : TagHelperBase
    {
        public CounterTagHelper(IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> options) 
            : base(httpContextAccessor, options) 
        {}

        [HtmlAttributeName("value")]
        public int Value { get; set; }

        [HtmlAttributeName("endpoint")]
        public string Endpoint { get; set; } = "/api/counter/update";

        protected override async Task<string> RenderHtmlAsync(TagHelperContext context, TagHelperOutput output)
        {
            string rawHtml = await ReadOwnHtmlAsync();
            string processedHtml = rawHtml
                .Replace("{counter_value}", Value.ToString())
                .Replace("{counter_display}", Value.ToString());

            output.Attributes.Add("endpoint", Endpoint);

            return processedHtml;
        }
    }
}
