using System.Net.NetworkInformation;
using NRpiMonitor.Database.Repositories;
using Prometheus;

namespace NRpiMonitor.Services;

public class PingService : IDisposable
{
    private const string Min = nameof(Min);
    private const string Avg = nameof(Avg);
    private const string Max = nameof(Max);
    
    private static readonly Gauge PingSuccess = Metrics.CreateGauge("ping_success", "Ping success rate", "host");
    private static readonly Gauge RoundTripTime = Metrics.CreateGauge("round_trip_time", "Roundtrip time per kind per host", "host", "kind");
    
    private readonly PingResultsRepository _repo;
    private readonly ILogger<PingService> _logger;
    private readonly Ping _pinger;
    public PingService(PingResultsRepository repo, ILogger<PingService> logger)
    {
        _repo = repo;
        _logger = logger;
        _pinger = new Ping();
    }

    public async Task<PingCheckResult> PingHost(string host, int num, DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var success = 0;
        var rtts = new List<long>(num);
        for (int i = 0; i < num; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var result = _pinger.Send(host);
                if (result.Status == IPStatus.Success)
                {
                    rtts.Add(result.RoundtripTime);
                    success++;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed ping attempt");
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

    public void Dispose()
    {
        _pinger.Dispose();
    }
}
