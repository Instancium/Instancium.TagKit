using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;


namespace Instancium.TagKit.Core.Controllers
{
    [Route("instancium/resources")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        [HttpGet("style-{hash}.css")]
        public IActionResult GetStyle(string hash)
        {
            var css = ResourceRegistry.GetStyle(hash);
            return css is null
                ? NotFound()
                : Content(css, "text/css");
        }

        [HttpGet("script-{hash}.js")]
        public IActionResult GetScript(string hash)
        {
            var js = ResourceRegistry.GetScript(hash);
            return js is null
                ? NotFound()
                : Content(js, "application/javascript");
        }

        [HttpGet("inst.js")]
        public IActionResult GetInstRuntime()
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Instancium.TagKit.Core.Runtime.EmbeddedResources.Inst.js");

            if (stream is null)
                return NotFound();

            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            return Content(content, "application/javascript");
        }
    }

}
