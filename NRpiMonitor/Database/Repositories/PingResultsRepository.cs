using Microsoft.EntityFrameworkCore;
using NRpiMonitor.Database.Model;
using NRpiMonitor.Services;

namespace NRpiMonitor.Database.Repositories;

public class PingResultsRepository
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public PingResultsRepository(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Task AddResult(PingCheckResult result)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Pings.Add(Map2Dal(result));
        return context.SaveChangesAsync();
    }
    
    public async Task<List<PingCheckResult>> GetLastState()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var maxDate = await context.Pings.Select(x => x.Timestamp).MaxAsync();
        var results = await context.Pings.Where(x => x.Timestamp == maxDate).ToListAsync();
        return results.Select(Map2Biz).ToList();
    }
    
    public async Task<List<PingCheckResult>> GetAllResults(DateTime notbefore)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var results = await context.Pings.Where(x => x.Timestamp >= notbefore).ToListAsync();
        return results.Select(Map2Biz).ToList();
    }

    private PingResultDal Map2Dal(PingCheckResult src) => 
        new()
        {
            Host = src.Host, Timestamp = src.Timestamp,
            SentPackets = src.TotalCount, ReceivedPackets = src.SuccessCount,
            MinRtt = src.MinRtt, AvgRtt = src.AvgRtt, MaxRtt = src.MaxRtt
        };

    private PingCheckResult Map2Biz(PingResultDal src) =>
        new(src.Timestamp, src.Host, src.SentPackets, src.ReceivedPackets, src.AvgRtt, src.MinRtt, src.MaxRtt);
}
