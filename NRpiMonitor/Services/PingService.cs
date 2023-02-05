using System.Net.NetworkInformation;
using NRpiMonitor.Database.Repositories;
using Prometheus;

namespace NRpiMonitor.Services;

public class PingService
{
    private const string Min = nameof(Min);
    private const string Avg = nameof(Avg);
    private const string Max = nameof(Max);
    
    private static readonly Gauge PingSuccess = Metrics.CreateGauge("ping_success", "Ping success rate", "host");
    private static readonly Gauge RoundTripTime = Metrics.CreateGauge("round_trip_time", "Roundtrip time per kind per host", "host", "kind");
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
        RoundTripTime.WithLabels(result.Host, Min).Set(result.MinRtt);
        RoundTripTime.WithLabels(result.Host, Avg).Set(result.AvgRtt);
        RoundTripTime.WithLabels(result.Host, Max).Set(result.MaxRtt);
    }

    public Task<List<PingCheckResult>> GetLastState() => _repo.GetLastState();
    public Task<List<PingCheckResult>> GetResults(DateTime notBefore) => _repo.GetAllResults(notBefore);
}
