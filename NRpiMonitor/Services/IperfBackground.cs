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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _bandwidthService.RunIperf3();
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Failed to run iperf3 service");
            }
            await Task.Delay(_cooldown, stoppingToken);
        }
    }
}
