namespace NRpiMonitor.Database.Model;

public class PingResultDal
{
    public int Id { get; set; }
    public string Host { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int SentPackets { get; set; }
    public int ReceivedPackets { get; set; }
    public double MinRtt { get; set; }
    public double AvgRtt { get; set; }
    public double MaxRtt { get; set; }
}
