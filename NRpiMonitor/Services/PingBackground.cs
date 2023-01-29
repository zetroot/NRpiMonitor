using Microsoft.Extensions.Options;
using NRpiMonitor.Services.Models;

namespace NRpiMonitor.Services;

public class PingBackground : BackgroundService
{
    private readonly PingService _service;
    private readonly List<string> _targetHosts;
    private readonly ILogger<PingBackground> _logger;

    public PingBackground(PingService service, IOptions<PingTargets> opts, ILogger<PingBackground> logger)
    {
        _targetHosts = opts.Value.Hosts;
        _service = service;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var datetime = DateTime.Now;
                foreach (var targetHost in _targetHosts)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await _service.PingHost(targetHost, 30, datetime, stoppingToken);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Something failed when pinging");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
