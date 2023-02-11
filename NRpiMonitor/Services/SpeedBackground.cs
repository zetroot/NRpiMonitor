namespace NRpiMonitor.Services;

public class SpeedBackground : BackgroundService
{
    private readonly SpeedtestService _speedtestService;
    private readonly ILogger<SpeedBackground> _logger;

    public SpeedBackground(SpeedtestService speedtestService, ILogger<SpeedBackground> logger)
    {
        _speedtestService = speedtestService;
        _logger = logger;
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
            
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
