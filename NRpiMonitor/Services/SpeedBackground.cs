using NRpiMonitor.Services.iperf;

namespace NRpiMonitor.Services;

public class SpeedBackground : BackgroundService
{
    private readonly SpeedtestService _speedtestService;
    private readonly BandwidthService _bandwidthService;
    private readonly ILogger<SpeedBackground> _logger;

    public SpeedBackground(SpeedtestService speedtestService, ILogger<SpeedBackground> logger, BandwidthService bandwidthService)
    {
        _speedtestService = speedtestService;
        _logger = logger;
        _bandwidthService = bandwidthService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _speedtestService.RunSpeedtest();
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Failed to do speedtest");
            }
            
            try
            {
                await _bandwidthService.RunIperf3();
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Failed to run iperf3 service");
            }
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
