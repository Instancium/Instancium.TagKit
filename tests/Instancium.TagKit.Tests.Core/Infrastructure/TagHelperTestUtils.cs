using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public static class TagHelperTestUtils
    {
        public static TagHelperContext CreateContext() =>
            new TagHelperContext(
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object?>(),
                uniqueId: Guid.NewGuid().ToString()
            );

        public static TagHelperOutput CreateOutput(string tagName) =>
            new TagHelperOutput(
                tagName,
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
            );
    }

}
