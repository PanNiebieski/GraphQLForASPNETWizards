using Microsoft.EntityFrameworkCore;

namespace _05RestAPIDocumentViews;

public class DocumentViewService : IDocumentViewService
{
    private readonly ApplicationDbContext _context;

    public DocumentViewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DocumentViewStatistics>> GetDocumentViewsOverTimeAsync(string documentId, TimeFrame timeFrame)
    {
        var today = DateTime.ParseExact("12-03-2025", "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

        switch (timeFrame)
        {
            case TimeFrame.Week:
                return await GetWeeklyStatisticsAsync(documentId, today.AddDays(-7), today);

            case TimeFrame.Month:
                return await GetMonthlyStatisticsAsync(documentId, today.AddMonths(-1), today);

            case TimeFrame.ThreeMonths:
                return await GetStatisticsByMonth3Async(documentId, today.AddMonths(-3), today);

            default:
                return await GetMonthlyStatisticsAsync(documentId, today.AddMonths(-1), today);
        }
    }

    private async Task<IEnumerable<DocumentViewStatistics>> GetWeeklyStatisticsAsync(string documentId, DateTime startDate, DateTime endDate)
    {
        // For a week view, we'll show daily data
        var result = new List<DocumentViewStatistics>();

        var dailyDataQuery = await _context.DocumentViews
            .Where(dv => dv.DocumentId == documentId && dv.Date >= startDate && dv.Date <= endDate)
            .ToListAsync();

        // Group by day manually
        var dailyData = dailyDataQuery
            .GroupBy(dv => dv.Date.Date)
            .Select(g => new
            {
                Date = g.Key,
                Views = g.Sum(x => x.Views)
            })
            .OrderBy(x => x.Date)
            .ToList();

        // Fill in any missing days
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var dayData = dailyData.FirstOrDefault(d => d.Date == date);
            result.Add(new DocumentViewStatistics
            {
                DocumentId = documentId,
                Date = date,
                Views = dayData?.Views ?? 0
            });
        }

        return result;
    }

    private async Task<IEnumerable<DocumentViewStatistics>> GetMonthlyStatisticsAsync(string documentId, DateTime startDate, DateTime endDate)
    {
        // For a month view, we'll aggregate data by week
        var result = new List<DocumentViewStatistics>();

        var weeklyDataQuery = await _context.DocumentViews
            .Where(dv => dv.DocumentId == documentId && dv.Date >= startDate && dv.Date <= endDate)
            .ToListAsync();

        // Calculate the start of the week (assuming Sunday as the first day of the week)
        var weekStart = startDate.Date.AddDays(-(int)startDate.DayOfWeek);

        // Group by week manually
        var weeklyData = new List<(DateTime WeekStart, int Views)>();

        while (weekStart <= endDate)
        {
            var weekEnd = weekStart.AddDays(7);
            var views = weeklyDataQuery
                .Where(dv => dv.Date >= weekStart && dv.Date < weekEnd)
                .Sum(dv => dv.Views);

            weeklyData.Add((weekStart, views));
            weekStart = weekEnd;
        }

        // Convert to DocumentViewStatistics
        foreach (var (week, views) in weeklyData)
        {
            result.Add(new DocumentViewStatistics
            {
                DocumentId = documentId,
                Date = week,
                Views = views
            });
        }

        return result;
    }

    private async Task<IEnumerable<DocumentViewStatistics>> GetStatisticsByMonth3Async(string documentId, DateTime startDate, DateTime endDate)
    {
        var result = new List<DocumentViewStatistics>();
        var monthlyDataQuery = await _context.DocumentViews
            .Where(dv => dv.DocumentId == documentId && dv.Date >= startDate && dv.Date <= endDate)
            .ToListAsync();

        // Calculate the start of the month
        var monthStart = new DateTime(startDate.Year, startDate.Month, 1);

        // Group by month manually
        var monthlyData = new List<(DateTime MonthStart, int Views)>();
        while (monthStart <= endDate)
        {
            var monthEnd = monthStart.AddMonths(1);
            var views = monthlyDataQuery
                .Where(dv => dv.Date >= monthStart && dv.Date < monthEnd)
                .Sum(dv => dv.Views);
            monthlyData.Add((monthStart, views));
            monthStart = monthEnd;
        }

        // Convert to DocumentViewStatistics
        foreach (var (month, views) in monthlyData)
        {
            result.Add(new DocumentViewStatistics
            {
                DocumentId = documentId,
                Date = month,
                Views = views
            });
        }

        return result;
    }
}
