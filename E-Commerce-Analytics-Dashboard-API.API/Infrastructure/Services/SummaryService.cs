using E_Commerce_Analytics_Dashboard_API.API.Application.Interfaces;
using E_Commerce_Analytics_Dashboard_API.API.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

public class SummaryService : ISummaryService
{
    private readonly ShopContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<SummaryService> _logger;

    public SummaryService(ShopContext context, IDistributedCache cache, ILogger<SummaryService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<(DateTime StartDate, DateTime EndDate)> GetFullReportingPeriodAsync()
    {
        const string cacheKey = "full_reporting_period";

        var cachedValue = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            var parts = cachedValue.Split('|');
            if (DateTime.TryParse(parts[0], out var start) && DateTime.TryParse(parts[1], out var end))
                return (start, end);
        }

        var startDate = await _context.Orders.MinAsync(o => o.Date);
        var endDate = await _context.Orders.MaxAsync(o => o.Date);

        await _cache.SetStringAsync(cacheKey, $"{startDate:O}|{endDate:O}", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        _logger.LogInformation("Calculated full reporting period: {Start} - {End}", startDate, endDate);
        return (startDate, endDate);
    }

    public async Task<decimal> GetAverageReceiptAsync(DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"average_receipt_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        var cachedValue = await _cache.GetStringAsync(cacheKey);
        if (cachedValue != null && decimal.TryParse(cachedValue, out var average))
            return average;

        var result = await _context.Orders
            .Where(o => o.Date >= startDate && o.Date <= endDate)
            .GroupBy(o => 1)
            .Select(g => new { TotalAmount = g.Sum(x => x.TotalAmount), Count = g.Count() })
            .FirstOrDefaultAsync();

        decimal averageCheck = (result?.Count ?? 0) > 0 ? result.TotalAmount / result.Count : 0;

        await _cache.SetStringAsync(cacheKey, averageCheck.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return averageCheck;
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"total_revenue_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        var cachedValue = await _cache.GetStringAsync(cacheKey);
        if (cachedValue != null && decimal.TryParse(cachedValue, out var totalRevenue))
            return totalRevenue;

        var result = await _context.Orders
            .Where(o => o.Date >= startDate && o.Date <= endDate)
            .SumAsync(o => o.TotalAmount);

        result = result > 0 ? result : 0;

        await _cache.SetStringAsync(cacheKey, result.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return result;
    }

    public async Task<(int ProductId, string Name, int TotalSold)> GetMostPopularProductAsync(DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"most_popular_product_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        var cachedValue = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            var parts = cachedValue.Split('|');
            if (int.TryParse(parts[0], out var productId) && int.TryParse(parts[2], out var totalSold))
                return (productId, parts[1], totalSold);
        }

        var result = await _context.OrderItems
            .Where(oi => oi.Order.Date >= startDate && oi.Order.Date <= endDate)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new { g.Key.ProductId, g.Key.Name, TotalSold = g.Sum(x => x.Quantity) })
            .OrderByDescending(o => o.TotalSold)
            .FirstOrDefaultAsync();

        if (result == null) return (0, string.Empty, 0);

        await _cache.SetStringAsync(cacheKey, $"{result.ProductId}|{result.Name}|{result.TotalSold}", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return (result.ProductId, result.Name, result.TotalSold);
    }

    public async Task<int> GetNumberOfOrdersAsync(DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"number_of_orders_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        var cachedValue = await _cache.GetStringAsync(cacheKey);
        if (cachedValue != null && int.TryParse(cachedValue, out var count))
            return count;

        var result = await _context.Orders
            .Where(o => o.Date >= startDate && o.Date <= endDate)
            .CountAsync();

        await _cache.SetStringAsync(cacheKey, result.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return result;
    }
}
