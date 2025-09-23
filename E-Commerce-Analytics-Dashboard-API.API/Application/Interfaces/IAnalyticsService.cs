namespace E_Commerce_Analytics_Dashboard_API.API.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<(decimal Current, decimal Previous, decimal ChangePercent)> PercentCompareWithPreviousPeriodAsync(DateTime startDate, DateTime endDate, string granularity);
        Task<(decimal Current, decimal Previous, decimal ChangeAbsolut)> AbsoluteCompareWithPreviousPeriodAsync(DateTime startDate, DateTime endDate, string granularity);
    }
}
