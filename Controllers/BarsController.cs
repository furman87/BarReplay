namespace BarReplay.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/bars/{interval:int}")]
    public class BarsController : ControllerBase
    {
        private readonly BarService _barService;

        public BarsController(BarService barService)
        {
            _barService = barService;
        }

        [HttpGet("init")]
        public async Task<IActionResult> GetInitialBars(int interval, [FromQuery] DateTime start, [FromQuery] int count = 200)
        {
            var bars = await _barService.GetInitialBars(start, interval, count);
            return Ok(bars);
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextBar(int interval, [FromQuery] DateTime after)
        {
            var bar = await _barService.GetNextBar(after, interval);
            return Ok(bar == null ? new object[0] : new[] { bar });
        }

        [HttpGet("before")]
        public async Task<IActionResult> GetBarsBefore(int interval, [FromQuery] DateTime time, [FromQuery] int count = 200)
        {
            var bars = await _barService.GetBarsBefore(time, interval, count);
            return Ok(bars);
        }

        [HttpGet("test")]
        public IActionResult Test() => Ok("Route working");
    }
}
