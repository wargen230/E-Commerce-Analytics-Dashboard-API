using E_Commerce_Analytics_Dashboard_API.API.Application.Interfaces;
using E_Commerce_Analytics_Dashboard_API.API.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/summary")]
public class SummaryController : ControllerBase
{
    private readonly ISummaryService _summaryService;
    private readonly IAnalyticsService _analyticsService;

    public SummaryController(ISummaryService summaryService, IAnalyticsService analyticsService)
    {
        _summaryService = summaryService;
        _analyticsService = analyticsService;
    }

    [HttpGet("full-reporting-period")]
    public async Task<IActionResult> FullReportingPeriod()
    {
        var (start, end) = await _summaryService.GetFullReportingPeriodAsync();
        return Ok(new { StartDate = start, EndDate = end });
    }

    [HttpGet("average-receipt")]
    public async Task<IActionResult> AverageReceipt([FromQuery] DateTime startDate, DateTime endDate)
    {
        var average = await _summaryService.GetAverageReceiptAsync(startDate, endDate);
        return Ok(new { Period = $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}", AverageReceipt = average });
    }

    [HttpGet("total-revenue")]
    public async Task<IActionResult> TotalRevenue([FromQuery] DateTime startDate, DateTime endDate)
    {
        var total = await _summaryService.GetTotalRevenueAsync(startDate, endDate);
        return Ok(new { Period = $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}", TotalRevenue = total });
    }

    [HttpGet("most-popular-product")]
    public async Task<IActionResult> MostPopularProduct([FromQuery] DateTime startDate, DateTime endDate)
    {
        var result = await _summaryService.GetMostPopularProductAsync(startDate, endDate);
        return Ok(new { result.ProductId, result.Name, result.TotalSold });
    }

    [HttpGet("number-of-orders")]
    public async Task<IActionResult> NumberOfOrders([FromQuery] DateTime startDate, DateTime endDate)
    {
        var count = await _summaryService.GetNumberOfOrdersAsync(startDate, endDate);
        return Ok(new { Period = $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}", Count = count });
    }

    [HttpGet("comparison-precent")]
    public async Task<IActionResult> ComparisonPrecent([FromQuery] DateTime startDate, DateTime endDate, string granularity)
    {
        var (current, previous, percent) = await _analyticsService.PercentCompareWithPreviousPeriodAsync(startDate, endDate, granularity);

        DateTime startDatePrevious, endDatePrevious;
        AnalyticsService.GetStartDatePreviousAndEndDatePrevious(startDate, endDate, granularity, out startDatePrevious, out endDatePrevious);

        return Ok(new
        {
            CurrentPeriod = new { Start = startDate, End = endDate, Revenue = current },
            PreviousPeriod = new { Start = startDatePrevious, End = endDatePrevious, Revenue = previous },
            Change = new { Percent = percent }
        });
    }
    [HttpGet("comparison-absolute")]
    public async Task<IActionResult> ComparisonAbsolute([FromQuery] DateTime startDate, DateTime endDate, string granularity)
    {
        var (current, previous, absolute) = await _analyticsService.PercentCompareWithPreviousPeriodAsync(startDate, endDate, granularity);

        DateTime startDatePrevious, endDatePrevious;
        AnalyticsService.GetStartDatePreviousAndEndDatePrevious(startDate, endDate, granularity, out startDatePrevious, out endDatePrevious);

        return Ok(new
        {
            CurrentPeriod = new { Start = startDate, End = endDate, Revenue = current },
            PreviousPeriod = new { Start = startDatePrevious, End = endDatePrevious, Revenue = previous },
            Change = new { Absolute = absolute }
        });
    }
}
