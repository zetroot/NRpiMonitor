namespace NRpiMonitor.Services;

public class SpeedBackground : BackgroundService
{
    private readonly SpeedtestService _service;
    private readonly ILogger<SpeedBackground> _logger;

    public SpeedBackground(SpeedtestService service, ILogger<SpeedBackground> logger)
    {
        _service = service;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _service.RunSpeedtest();
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Failed to do speedtest");
            }
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
