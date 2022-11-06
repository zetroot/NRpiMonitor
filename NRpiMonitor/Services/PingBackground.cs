namespace NRpiMonitor.Services;

public class PingBackground : BackgroundService
{
    private readonly PingService _service;

    public PingBackground(PingService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var datetime = DateTime.Now;
                await _service.PingHost("192.168.88.1", 30, datetime);
                await _service.PingHost("1.1.1.1", 30, datetime);
                await _service.PingHost("8.8.8.8", 30, datetime);
            }
            catch{}

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
