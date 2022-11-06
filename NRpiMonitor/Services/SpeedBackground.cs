namespace NRpiMonitor.Services;

public class SpeedBackground : BackgroundService
{
    private readonly SpeedtestService _service;

    public SpeedBackground(SpeedtestService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _service.RunSpeedtest();
            }
            catch
            {
                
            }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
