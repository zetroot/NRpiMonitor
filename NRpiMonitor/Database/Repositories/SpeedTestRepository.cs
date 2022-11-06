using Microsoft.EntityFrameworkCore;
using NRpiMonitor.Services.Models.Speedtest;

namespace NRpiMonitor.Database.Repositories;

public class SpeedTestRepository
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public SpeedTestRepository(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Task AddResult(SpeedtestResultJson resultJson)
    {
        using var context = _contextFactory.CreateDbContext();
        context.SpeedTests.Add(new(){DownloadBandwidth = resultJson.Download.Bandwidth, UploadBandwidth = resultJson.Upload.Bandwidth, Timestamp = DateTime.Now});
        return context.SaveChangesAsync();
    }
    
    public async Task<SpeedtestResult> GetLastState()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var maxDate = await context.SpeedTests.Select(x => x.Timestamp).MaxAsync();
        var result = await context.SpeedTests.FirstOrDefaultAsync(x => x.Timestamp == maxDate);
        return new() { Timestamp = result.Timestamp, Download = result.DownloadBandwidth/1024.0/1024.0, Upload = result.UploadBandwidth/1024.0/1024.0 };
    }
    
    public async Task<List<SpeedtestResult>> GetAllResults(DateTime notbefore)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var results = await context.SpeedTests.Where(x => x.Timestamp >= notbefore).ToListAsync();
        return results.Select(x => new SpeedtestResult(){Timestamp = x.Timestamp, Download = x.DownloadBandwidth/1024.0/1024.0, Upload = x.UploadBandwidth/1024.0/1024.0}).ToList();
    }
}
