using Microsoft.AspNetCore.Mvc;

namespace Instancium.TagKit.DemoApp.Components.Counter
{

    [Route("api/[controller]")]
    [ApiController]
    public class CounterController : ControllerBase
    {
        public class CounterRequest
        {
            public int Value { get; set; }
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody] CounterRequest data)
        {
            int delta = 9;
            int newValue = data.Value + delta;

            return Ok(new
            {
                delta,
                result = newValue
            });
        }
    }
}
