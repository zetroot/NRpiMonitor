using System.Net.NetworkInformation;
using NRpiMonitor.Database.Repositories;

namespace NRpiMonitor.Services;

public class PingService
{
    private readonly PingResultsRepository _repo;

    public PingService(PingResultsRepository repo)
    {
        _repo = repo;
    }

    public async Task<PingCheckResult> PingHost(string host, int num, DateTime timestamp)
    {
        using var ping = new Ping();
        var success = 0;
        var rtts = new List<long>(num);
        for (int i = 0; i < num; ++i)
        {
            var result = await ping.SendPingAsync(host);
            if (result.Status == IPStatus.Success)
            {
                rtts.Add(result.RoundtripTime);
                success++;
            }
        }

        
        var pingRes = new PingCheckResult(timestamp, host, num, success, rtts.Sum(x => (double)x) / success, rtts.Min(x => (double)x), rtts.Max(x => (double)x));
        await _repo.AddResult(pingRes);
        return pingRes;
    }

    public Task<List<PingCheckResult>> GetLastState() => _repo.GetLastState();
    public Task<List<PingCheckResult>> GetResults(DateTime notBefore) => _repo.GetAllResults(notBefore);
}
