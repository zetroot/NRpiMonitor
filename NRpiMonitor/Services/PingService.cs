using System.Net.NetworkInformation;
using NRpiMonitor.Database.Repositories;
using Prometheus;

namespace NRpiMonitor.Services;

public class PingService
{
    private static readonly Gauge PingSuccess = Metrics.CreateGauge("ping_success", "Ping success rate", "host");
    private static readonly Gauge PingMin = Metrics.CreateGauge("ping_min", "Ping min time", "host");
    private static readonly Gauge PingAvg = Metrics.CreateGauge("ping_avg", "Ping avg time", "host");
    private static readonly Gauge PingMax = Metrics.CreateGauge("ping_max", "Ping max time", "host");
    private readonly PingResultsRepository _repo;

    public PingService(PingResultsRepository repo)
    {
        _repo = repo;
    }

    public async Task<PingCheckResult> PingHost(string host, int num, DateTime timestamp, CancellationToken cancellationToken = default)
    {
        using var ping = new Ping();
        var success = 0;
        var rtts = new List<long>(num);
        for (int i = 0; i < num; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await ping.SendPingAsync(host);
            if (result.Status == IPStatus.Success)
            {
                rtts.Add(result.RoundtripTime);
                success++;
            }
        }
        
        var pingRes = new PingCheckResult(timestamp,
            host,
            num,
            success,
            rtts.Sum(x => (double)x) / success,
            rtts.Min(x => (double)x),
            rtts.Max(x => (double)x));
        ExposeResult(pingRes);
        await _repo.AddResult(pingRes);
        return pingRes;
    }

    private static void ExposeResult(PingCheckResult result)
    {
        PingSuccess.WithLabels(result.Host).Set((double)result.SuccessCount/result.TotalCount);
        PingMin.WithLabels(result.Host).Set(result.MinRtt);
        PingAvg.WithLabels(result.Host).Set(result.AvgRtt);
        PingMax.WithLabels(result.Host).Set(result.MaxRtt);
    }

    public Task<List<PingCheckResult>> GetLastState() => _repo.GetLastState();
    public Task<List<PingCheckResult>> GetResults(DateTime notBefore) => _repo.GetAllResults(notBefore);
}
