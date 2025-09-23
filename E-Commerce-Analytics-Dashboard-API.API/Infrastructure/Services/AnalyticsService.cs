using E_Commerce_Analytics_Dashboard_API.API.Application.Interfaces;
using E_Commerce_Analytics_Dashboard_API.API.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace E_Commerce_Analytics_Dashboard_API.API.Infrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ShopContext _context;
        private readonly ILogger<AnalyticsService> _logger;
        private readonly IDistributedCache _cache;
        public AnalyticsService(ILogger<AnalyticsService> logger, IDistributedCache cache, ShopContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }

        public async Task<(decimal Current, decimal Previous, decimal ChangeAbsolut)> AbsoluteCompareWithPreviousPeriodAsync(DateTime startDate, DateTime endDate, string granularity)
        {
            string cacheKey = $"timeseries_{granularity}absolute_comparison_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                var parts = cachedValue.Split('|');
                if (decimal.TryParse(parts[0], out var currentRevenue) &&
                    decimal.TryParse(parts[1], out var previousRevenue) &&
                    decimal.TryParse(parts[2], out var changeAbsolute))
                    return (currentRevenue, previousRevenue, changeAbsolute);
            }

            DateTime startDatePrevious, endDatePrevious;
            GetStartDatePreviousAndEndDatePrevious(startDate, endDate, granularity, out startDatePrevious, out endDatePrevious);

            var sums = await _context.Orders
                .Where(o => (o.Date >= startDate && o.Date <= endDate) ||
                            (o.Date >= startDatePrevious && o.Date <= endDatePrevious))
                .Select(o => new { Period = o.Date >= startDate && o.Date <= endDate ? "current" : "previous", o.TotalAmount })
                .GroupBy(x => x.Period)
                .Select(g => new { Period = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            var resultCurrent = sums.FirstOrDefault(s => s.Period == "current")?.Total ?? 0m;
            var resultPrevious = sums.FirstOrDefault(s => s.Period == "previous")?.Total ?? 0m;

            decimal _changeAbsolute = resultCurrent - resultPrevious;

            await _cache.SetStringAsync(cacheKey, $"{resultCurrent}|{resultPrevious}|{_changeAbsolute}", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return (resultCurrent, resultPrevious, _changeAbsolute);

        }

        public async Task<(decimal Current, decimal Previous, decimal ChangePercent)> PercentCompareWithPreviousPeriodAsync(DateTime startDate, DateTime endDate, string granularity)
        {
            string cacheKey = $"precent_comparison_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                var parts = cachedValue.Split('|');
                if (decimal.TryParse(parts[0], out var currentRevenue) &&
                    decimal.TryParse(parts[1], out var previousRevenue) &&
                    decimal.TryParse(parts[2], out var changePercent))
                    return (currentRevenue, previousRevenue, changePercent);
            }

            DateTime startDatePrevious, endDatePrevious;
            GetStartDatePreviousAndEndDatePrevious(startDate, endDate, granularity, out startDatePrevious, out endDatePrevious);

            var sums = await _context.Orders
                .Where(o => (o.Date >= startDate && o.Date <= endDate) ||
                            (o.Date >= startDatePrevious && o.Date <= endDatePrevious))
                .Select(o => new { Period = o.Date >= startDate && o.Date <= endDate ? "current" : "previous", o.TotalAmount })
                .GroupBy(x => x.Period)
                .Select(g => new { Period = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            var resultCurrent = sums.FirstOrDefault(s => s.Period == "current")?.Total ?? 0m;
            var resultPrevious = sums.FirstOrDefault(s => s.Period == "previous")?.Total ?? 0m;

            decimal _changePercent = resultPrevious > 0 ? (resultCurrent - resultPrevious) / resultPrevious * 100 : 0;

            await _cache.SetStringAsync(cacheKey, $"{resultCurrent}|{resultPrevious}|{_changePercent}", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return (resultCurrent, resultPrevious, _changePercent);
        }

        public static void GetStartDatePreviousAndEndDatePrevious(DateTime startDate, DateTime endDate, string granularity, out DateTime startDatePrevious, out DateTime endDatePrevious)
        {
            switch (granularity.ToLower())
            {
                case "day":
                    startDatePrevious = startDate.AddDays(-(endDate - startDate).Days - 1);
                    endDatePrevious = endDate.AddDays(-(endDate - startDate).Days - 1);
                    break;
                case "week":
                    startDatePrevious = startDate.AddDays(-7);
                    endDatePrevious = endDate.AddDays(-7);
                    break;
                case "month":
                    startDatePrevious = startDate.AddMonths(-1);
                    endDatePrevious = endDate.AddMonths(-1);
                    break;
                case "year":
                    startDatePrevious = startDate.AddYears(-1);
                    endDatePrevious = endDate.AddYears(-1);
                    break;
                default:
                    throw new ArgumentException("Unknown granularity");
            }
        }
    }
}
