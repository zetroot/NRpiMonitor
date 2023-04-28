using Microsoft.Extensions.Options;
using NRpiMonitor.Services.iperf;
using NRpiMonitor.Services.iperf.Models;

namespace NRpiMonitor.Services;

public class IperfBackground : BackgroundService
{
    private readonly BandwidthService _bandwidthService;
    private readonly ILogger<IperfBackground> _logger;
    private readonly TimeSpan _cooldown;

    public IperfBackground(BandwidthService bandwidthService, IOptions<IperfSettings> opts, ILogger<IperfBackground> logger)
    {
        _bandwidthService = bandwidthService;
        _logger = logger;
        _cooldown = opts.Value.Cooldown;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Started iperf3 bandwidth monitoring service");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("Running next turn of bw monitoring");
            try
            {
                await _bandwidthService.RunIperf3();
                _logger.LogDebug("Finished iperf3 bw monitoring turn");
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Failed to run iperf3 service");
            }
            await Task.Delay(_cooldown, stoppingToken);
        }
    }
}
