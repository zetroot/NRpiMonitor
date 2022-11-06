namespace NRpiMonitor.Services.Models.Speedtest;

public class SpeedtestResult
{
    public DateTime Timestamp { get; set; }
    public double Upload { get; set; }
    public double Download { get; set; }
}
