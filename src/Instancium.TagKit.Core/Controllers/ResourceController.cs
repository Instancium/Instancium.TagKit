using Instancium.TagKit.Core.Core;
using Instancium.TagKit.Core.Utils;
using Microsoft.AspNetCore.Mvc;


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
    }

}
