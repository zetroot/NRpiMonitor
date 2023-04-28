using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NRpiMonitor.Services.iperf.Models;
using Prometheus;

namespace NRpiMonitor.Services.iperf;

public class BandwidthService
{
    private const string Download = nameof(Download);
    private const string Upload = nameof(Upload);
    private static readonly Gauge Speed = Metrics.CreateGauge("iperf3_bandwidth", "iperf3 results", "direction", "host"); 
    private static readonly Gauge Size = Metrics.CreateGauge("iperf3_size", "iperf3 transferred", "direction", "host");

    private readonly ILogger<BandwidthService> _logger;
    private readonly ICollection<IperfHost> _hosts;

    public BandwidthService(IOptions<IperfSettings> opts, ILogger<BandwidthService> logger)
    {
        _logger = logger;
        _hosts = opts.Value.Hosts;
    }
    
    public async Task RunIperf3()
    {
        foreach (var iperfHost in _hosts)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { { "Host", iperfHost.Host } }))
            {
                _logger.LogDebug("Starting test for next host");
                try
                {
                    var result = await TestHost(iperfHost.Host, iperfHost.Username, iperfHost.Password, iperfHost.RsaKeyfile);
                    ExposeResult(result, iperfHost.Host);
                    _logger.LogInformation("Test for host finished, metrics exposed");
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Monitoring failed for host, will retry later");
                }
            }
        }
    }

    private async Task<OutputModelRoot> TestHost(string host, string? user, string? password, string? keyfile)
    {
        ArgumentException.ThrowIfNullOrEmpty(host, nameof(host));
        var args = new StringBuilder("-J -c ").Append(host);
        if (!string.IsNullOrWhiteSpace(user))
            args.Append(" --username ").Append(user);
        if(!string.IsNullOrWhiteSpace(keyfile))
            args.Append(" --rsa-public-key-path ").Append(keyfile);

        var start = new ProcessStartInfo()
        {
            FileName = "iperf3",
            Arguments = args.ToString(),
            RedirectStandardOutput = true
        };
        if(!string.IsNullOrWhiteSpace(password))
            start.EnvironmentVariables.Add("IPERF3_PASSWORD", password);
        var proc = new Process { StartInfo = start };
        proc.Start();
        var output = await proc.StandardOutput.ReadToEndAsync();

        var result = JsonSerializer.Deserialize<OutputModelRoot>(output);
        _logger.LogDebug("iperf result: {@Result}", result);
        if (result is null)
        {
            _logger.LogError("iperf output cant be deserialized. Raw output: {RawOutput}", output);
            throw new InvalidOperationException("Iperf run failed");
        }

        return result;
    }

    private void ExposeResult(OutputModelRoot? result, string host)
    {
        if(result?.Summary?.Received?.Bandwidth is {} download)
            Speed.WithLabels(Download, host).Set(download);
        if(result?.Summary?.Sent?.Bandwidth is {} upload)
            Speed.WithLabels(Upload, host).Set(upload);
        
        if(result?.Summary?.Received?.Size is {} received)
            Size.WithLabels(Download, host).Set(received);
        if(result?.Summary?.Sent?.Size is {} sent) 
            Size.WithLabels(Upload, host).Set(sent);
    }
}
