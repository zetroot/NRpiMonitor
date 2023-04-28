using System.Diagnostics;
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
            _logger.LogDebug("Starting ping session for configured hosts");
            var sw = Stopwatch.StartNew();
            try
            {
                var datetime = DateTime.Now;
                foreach (var targetHost in _targetHosts)
                {
                    using (_logger.BeginScope(new Dictionary<string, object> { { "Host", targetHost } }))
                    {

                        stoppingToken.ThrowIfCancellationRequested();
                        await _service.PingHost(targetHost, 30, datetime, stoppingToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something failed when pinging");
            }
            finally
            {
                sw.Stop();
                _logger.LogDebug("Ping session finished! Took {Duration}", sw.Elapsed);
                
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
