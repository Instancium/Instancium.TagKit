
using Instancium.TagKit.Core.Config;
using Instancium.TagKit.Core.Samples.HelloTagHelper;
using Instancium.TagKit.Tests.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Instancium.TagKit.Tests.Core
{
    public class HelloTagHelperTests
    {

        [Fact]
        public async Task Should_Render_Localized_Html_With_CurrentTime()
        {
                var http = new FakeHttpContextAccessor("en");
                var tagHelper = new HelloTagHelper(http, FakeAppSettings.Instance);
                

            var context = TagHelperTestUtils.CreateContext();
            var output = TagHelperTestUtils.CreateOutput("inst-hello");

            await tagHelper.ProcessAsync(context, output);

            var content = output.Content.GetContent();

            Assert.Contains("Hello from Instancium!", content);
            Assert.Contains("Current time", content);
            Assert.Contains(DateTime.UtcNow.Year.ToString(), content); 
        }

        [Fact]
        public void Should_Replace_Missing_Key_With_Placeholder()
        {
            //var helper = new TestableHelloTagHelper();
            //helper.LoadTestLocalization("en");

            //var result = helper.LocalizeTest("<span>@(_MissingKey_)</span>");
            //Assert.Contains("[MISSING", result);
        }
    }

}

