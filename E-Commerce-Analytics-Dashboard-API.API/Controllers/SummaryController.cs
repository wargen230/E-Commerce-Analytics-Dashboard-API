using E_Commerce_Analytics_Dashboard_API.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Analytics_Dashboard_API.API.Controllers
{
    [ApiController]
    [Route("api/Summary")]
    public class SummaryController : ControllerBase
    {
        private readonly ShopContext _context;
        private ILogger<SummaryController> _logger;
        public SummaryController(ILogger<SummaryController> logger, ShopContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet("average-receipt")]
        public IActionResult AverageReceipt([FromQuery] DateTime startDate, DateTime endDate)
        {
            if (_context == null)
            {
                _logger.LogError("The database is not connected");
                return BadRequest("The database is not connected");
            }

            startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);

            var orders = _context.Orders
                .Where(o => o.Date >= startDate && o.Date <= endDate);

            var totalAmount = orders.Sum(o => o.TotalAmount);
            var count = orders.Count();

            decimal averageCheck = count > 0 ? totalAmount / count : 0;

            _logger.LogInformation(
                "The average check for the period has been calculated: {Start} - {End}",
                startDate.ToString("yyyy-MM-dd"),
                endDate.ToString("yyyy-MM-dd")
            );

            return Ok(new
            {
                Period = $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}",
                AverageReceipt = averageCheck
            });
        }
    }
}
