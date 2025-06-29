using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Core.Controllers
{
    [ApiController]
    [Route("taghelper")]
    public class TagHelperController : ControllerBase
    {
        private readonly IServiceProvider _provider;

        public TagHelperController(IServiceProvider provider)
        {
            _provider = provider;
        }

        [HttpGet("{tag}")]
        public async Task<IActionResult> RenderTag(string tag, [FromQuery] string lang = "en", [FromQuery] string? id = null)
        {
            var type = DiscoverTagHelperByName(tag);
            if (type == null)
                return NotFound($"TagHelper '{tag}' not found");

            var helper = ActivatorUtilities.CreateInstance(_provider, type) as TagHelperBase;
            if (helper == null)
                return BadRequest("Component failed to initialize");
            helper.LanguageCode = lang;
           
            if(id != null)
            helper.ElementId = id;

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>
                {
                    ["lang-code"] = lang
                },
                Guid.NewGuid().ToString());

            var output = new TagHelperOutput(tag,
                new TagHelperAttributeList(),
                (useCached, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            await helper.ProcessAsync(context, output);
            var html = output.Content.GetContent();

            return Content(html, "text/html");
        }

        private static Type? DiscoverTagHelperByName(string tag)
        {
            var candidates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                });

            return candidates.FirstOrDefault(t =>
                typeof(TagHelper).IsAssignableFrom(t) &&
                t.Name.Equals($"{ToPascal(tag)}TagHelper", StringComparison.OrdinalIgnoreCase));
        }

        private static string ToPascal(string kebab)
        {
            return string.Concat(
                kebab.Split('-', StringSplitOptions.RemoveEmptyEntries)
                     .Select(w => char.ToUpper(w[0]) + w.Substring(1))
            );
        }
    }

}
