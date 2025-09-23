using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Analytics_Dashboard_API.API.Application.Interfaces
{
    public interface ISummaryService
    {
        Task<(DateTime StartDate, DateTime EndDate)> GetFullReportingPeriodAsync();
        Task<decimal> GetAverageReceiptAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<(int ProductId, string Name, int TotalSold)> GetMostPopularProductAsync(DateTime startDate, DateTime endDate);
        Task<int> GetNumberOfOrdersAsync(DateTime startDate, DateTime endDate);
    }
}
