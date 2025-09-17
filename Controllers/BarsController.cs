namespace BarReplay.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/bars/{interval:int}")]
    public class BarsController : ControllerBase
    {
    private readonly BarService _barService;
    private readonly ILogger<BarsController> _logger;

        public BarsController(BarService barService, ILogger<BarsController> logger)
        {
            _barService = barService;
            _logger = logger;
        }

        [HttpGet("init")]
        public async Task<IActionResult> GetInitialBars(int interval, [FromQuery] DateTime start, [FromQuery] int count = 200)
        {
            _logger.LogInformation("Entered GetInitialBars with interval={interval}, start={start}, count={count}", interval, start, count);
            var bars = await _barService.GetInitialBars(start, interval, count);
            return Ok(bars);
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextBar(int interval, [FromQuery] DateTime after)
        {
            _logger.LogInformation("Entered GetNextBar with interval={interval}, after={after}", interval, after);
            var bar = await _barService.GetNextBar(after, interval);
            return Ok(bar == null ? new object[0] : new[] { bar });
        }

        [HttpGet("before")]
        public async Task<IActionResult> GetBarsBefore(int interval, [FromQuery] DateTime time, [FromQuery] int count = 200)
        {
            _logger.LogInformation("Entered GetBarsBefore with interval={interval}, time={time}, count={count}", interval, time, count);
            var bars = await _barService.GetBarsBefore(time, interval, count);
            return Ok(bars);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> GetRefreshBars(int interval)
        {
            _logger.LogInformation("Entered GetRefreshBars with interval={interval}", interval);
            await _barService.CreateOrRefreshView(interval);
            return Ok();
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("Entered Test endpoint");
            return Ok("Route working");
        }
    }
}
