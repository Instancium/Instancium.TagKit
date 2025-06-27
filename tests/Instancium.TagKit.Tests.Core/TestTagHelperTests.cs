using Instancium.TagKit.Core.TestTagHelper;
using Instancium.TagKit.Tests.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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





        [Fact]
        public async Task ProcessAsync_ShouldMatchSnapshot()
        {
            // Arrange
            var tagHelper = new TestTagHelper(
                new FakeHttpContextAccessor("en"),
                FakeAppSettings.Instance,
                new FakeLocalizerFactory());

            var context = TagHelperTestUtils.CreateContext();
            var output = TagHelperTestUtils.CreateOutput("test-tag");

            // Act
            await tagHelper.ProcessAsync(context, output);

            // Build full HTML
            string tagName = output.TagName;
            string id = output.Attributes["id"]?.Value?.ToString()!;
            string content = output.Content.GetContent();
            string fullHtml = $"<{tagName} id=\"{id}\">{content}</{tagName}>";

            // Normalize dynamic values
            fullHtml = fullHtml.Replace(DateTime.UtcNow.ToString("u"), "PLACEHOLDER");
            fullHtml = Regex.Replace(fullHtml, @"tag-[a-f0-9]{32}", "tag-PLACEHOLDER");

            // Assert snapshot
            SnapshotTestUtils.AssertMatchesSnapshot("TestTagHelper.snapshot.html", fullHtml);
        }
    }

}
