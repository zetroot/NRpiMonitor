using System.Diagnostics;
using System.Text.Json;
using NRpiMonitor.Database.Repositories;
using NRpiMonitor.Services.Models.Speedtest;
using Prometheus;

namespace NRpiMonitor.Services;

public class SpeedtestService
{
    private const string Download = nameof(Download);
    private const string Upload = nameof(Upload);
    
    private readonly SpeedTestRepository _repo;
    private readonly ILogger<SpeedtestService> _logger;
    private static readonly Gauge Speed = Metrics.CreateGauge("bandwidth", "Speedtest results", "direction");

    public SpeedtestService(SpeedTestRepository repo, ILogger<SpeedtestService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task RunSpeedtest()
    {
        var proc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "speedtest",
                Arguments = "--accept-license --format=json",
                RedirectStandardOutput = true
            }
        };
        proc.Start();
        var output = await proc.StandardOutput.ReadToEndAsync();
        var result = JsonSerializer.Deserialize<SpeedtestResultJson>(output);
        _logger.LogDebug("Speedtest result was {@ResultModel}", result);
        if (result is {Download: not null, Upload: not null })
        {
            ExposeResult(result);
            await _repo.AddResult(result);
        }
        else
        {
            _logger.LogWarning("Could not get valid speedtest result. Metrics will be cleared");
            ClearMetrics();
        }

    }

    private static void ClearMetrics()
    {
        Speed.RemoveLabelled(Download);
        Speed.RemoveLabelled(Upload);
    }
    
    private static void ExposeResult(SpeedtestResultJson result)
    {
        Speed.WithLabels(Download).Set(result.Download.Bandwidth);
        Speed.WithLabels(Upload).Set(result.Upload.Bandwidth);
    }

    public Task<SpeedtestResult> GetLastState() => _repo.GetLastState();
    public Task<List<SpeedtestResult>> GetResults(DateTime notBefore, DateTime notAfter) =>
        _repo.GetAllResults(notBefore, notAfter);
}
