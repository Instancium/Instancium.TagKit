using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

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
            HttpContext.Items["__inst_render_mode"] = "fragment";

            var type = ComponentRegistry.Resolve(tag);
            if (type == null)
                return NotFound($"TagHelper '{tag}' not found");

            var helper = ActivatorUtilities.CreateInstance(_provider, type) as TagHelperBase;
            if (helper == null)
                return BadRequest("Component failed to initialize");
            helper.LanguageCode = lang;

            var allAttributes = new TagHelperAttributeList
            {
                { "lang-code", lang }
            };

            if (id != null)
            {
                helper.ElementId = id;
                allAttributes.Add("id", id);
            }

            var context = new TagHelperContext(
                allAttributes: allAttributes,
                items: new Dictionary<object, object>(),
                uniqueId: Guid.NewGuid().ToString());

            var output = new TagHelperOutput(tag,
                new TagHelperAttributeList(),
                (useCached, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            await helper.ProcessAsync(context, output);
            var html = output.Content.GetContent();

            var manifest = ResourceManifest.Current;
            return new JsonResult(new
            {
                html,
                resources = new
                {
                    scripts = manifest.ScriptHashes.ToArray(),
                    styles = manifest.StyleHashes.ToArray(),
                },
            });

        }
    }

}
