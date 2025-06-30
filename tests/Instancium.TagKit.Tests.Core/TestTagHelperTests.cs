using Instancium.TagKit.Core.Samples.TestTagHelper;
using Instancium.TagKit.Tests.Core.Infrastructure;

namespace Instancium.TagKit.Tests.Core
{
    public class TestTagHelperTests
    {

        [Fact]
        public async Task ShouldResolve_LocalizerAndLanguageCode_FromContextOverride()
        {
            var http = new FakeHttpContextAccessor("fr"); // ← will be ignored
            var tagHelper = new TestTagHelper(http, FakeAppSettings.Instance, new FakeLocalizerFactory());

            var context = TagHelperTestUtils.CreateContext();
            context.Items["lang-code"] = "it"; // ← will be set
            var output = TagHelperTestUtils.CreateOutput("test-tag");

            await tagHelper.ProcessAsync(context, output);
            string content = output.Content.GetContent();

            // Check: the language replacement worked (this means the culture and localizer worked)
            Assert.Contains("[_ClickMe_]", content);
        }

    }

}
