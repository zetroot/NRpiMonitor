using System.Diagnostics;
using System.Text.Json;
using NRpiMonitor.Services.iperf.Models;
using NRpiMonitor.Services.Models.Speedtest;
using Prometheus;

namespace NRpiMonitor.Services.iperf;

public class BandwidthService
{
    private const string Download = nameof(Download);
    private const string Upload = nameof(Upload);
    private static readonly Gauge Speed = Metrics.CreateGauge("iperf3_bandwidth", "iperf3 results", "direction"); 
    private static readonly Gauge Size = Metrics.CreateGauge("iperf3_size", "iperf3 transferred", "direction");

    private readonly ILogger<BandwidthService> _logger;

    public BandwidthService(ILogger<BandwidthService> logger)
    {
        _logger = logger;
    }

    public async Task RunIperf3()
    {
        var proc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "iperf3",
                Arguments = "-J -c lg.vie.alwyzon.net",
                RedirectStandardOutput = true
            }
        };
        proc.Start();
        var output = await proc.StandardOutput.ReadToEndAsync();

        var result = JsonSerializer.Deserialize<OutputModelRoot>(output);
        _logger.LogDebug("iperf result: {@Result}", result);
        if (result is null)
        {
            _logger.LogDebug("iperf raw output: {RawOutput}", output);
        }
        ExposeResult(result);
    }

    private void ExposeResult(OutputModelRoot? result)
    {
        Speed.WithLabels(Download).Set(result?.Summary?.Received?.Bandwidth ?? 0);
        Speed.WithLabels(Upload).Set(result?.Summary?.Sent?.Bandwidth ?? 0);
        Size.WithLabels(Download).Set(result?.Summary?.Received?.Size ?? 0);
        Size.WithLabels(Upload).Set(result?.Summary?.Sent?.Size ?? 0);
    }
}
