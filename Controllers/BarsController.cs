namespace BarReplay.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class BarsController : ControllerBase
    {
        private readonly BarService _barService;

        public BarsController(BarService barService)
        {
            _barService = barService;
        }

        [HttpGet("init")]
        public async Task<IActionResult> GetInitialBars([FromQuery] DateTime start, [FromQuery] int count = 200)
        {
            var bars = await _barService.GetInitialBars(start, count);
            return Ok(bars);
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextBar([FromQuery] DateTime after)
        {
            var bar = await _barService.GetNextBar(after);
            return Ok(bar == null ? new object[0] : new[] { bar });
        }

        [HttpGet("before")]
        public async Task<IActionResult> GetBarsBefore([FromQuery] DateTime time, [FromQuery] int count = 200)
        {
            var bars = await _barService.GetBarsBefore(time, count);
            return Ok(bars);
        }

        [HttpGet("test")]
        public IActionResult Test() => Ok("Route working");
    }
}
