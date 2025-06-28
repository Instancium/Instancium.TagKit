using Instancium.TagKit.Core.Components.TestTagHelper;
using Instancium.TagKit.Core.Config;
using Instancium.TagKit.Core.Utils;
using Instancium.TagKit.Tests.Core.Infrastructure;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Instancium.TagKit.Tests.Core
{
    public class TagViewBuilderTests
    {
        [Fact]
        public async Task ShouldInline_CssAndJs_FromEmbeddedResources()
        {
            var builder = new TagViewBuilder { ComponentId = "x42" };
            var html = await builder.ReadFromResourceAsync(typeof(TestTagHelper));

            Assert.Contains("<style>", html);
            Assert.Contains(".hello-card", html);
            Assert.Contains("<script>", html);
        }

        [Fact]
        public void ShouldScopeCss_IfNotOptedOut()
        {
            var builder = new TagViewBuilder { ComponentId = "cmp-77" };
            string inputCss = ".block { color: red; }";
            
            var method = typeof(TagViewBuilder).GetMethod("ScopeCssToComponent", BindingFlags.NonPublic | BindingFlags.Instance);
            var scoped = (string) (method?.Invoke(builder, [inputCss]) ?? string.Empty);

            //string scoped = builder.InvokeCssScope(inputCss);
            Assert.Contains("#cmp-77 .block", scoped);
        }

        [Fact]
        public void ShouldSkipScope_IfDirectivePresent()
        {
            var builder = new TagViewBuilder { ComponentId = "cmp-any" };
            string inputCss = "/* @no-scope */\n.block { color: red; }";
            var method = typeof(TagViewBuilder).GetMethod("ScopeCssToComponent", BindingFlags.NonPublic | BindingFlags.Instance);
            var scoped = (string)(method?.Invoke(builder, [inputCss]) ?? string.Empty);

            //string result = builder.InvokeCssScope(inputCss);
            Assert.Contains("/* @no-scope */", scoped);
            Assert.DoesNotContain("#cmp-any", scoped);
        }

        [Fact]
        public async Task ShouldReturn_InnerMarkup_WithoutTagHelperWrapper()
        {
            var builder = new TagViewBuilder { ComponentId = "cmp-x" };
            var html = await builder.ReadFromResourceAsync(typeof(TestTagHelper));

            Assert.DoesNotContain("<tag-helper", html);
            Assert.Contains("hello-card", html); // Основной контент остался
        }

        [Fact]
        public async Task ProcessAsync_ShouldSetElementId_OnOutputTag()
        {
            // Arrange
            var http = new FakeHttpContextAccessor("en");
            var tagHelper = new TestTagHelper(http, FakeAppSettings.Instance, new FakeLocalizerFactory());

            var context = TagHelperTestUtils.CreateContext();
            var output = TagHelperTestUtils.CreateOutput("test-tag");

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Assert
            var idAttr = output.Attributes.FirstOrDefault(a => a.Name == "id");
            Assert.NotNull(idAttr);
            Assert.StartsWith("tag-", idAttr!.Value?.ToString());
        }
    }


}
