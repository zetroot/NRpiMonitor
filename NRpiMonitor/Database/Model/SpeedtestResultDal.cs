namespace NRpiMonitor.Database.Model;

public class SpeedtestResultDal
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int UploadBandwidth { get; set; }
    public int DownloadBandwidth { get; set; }
}
