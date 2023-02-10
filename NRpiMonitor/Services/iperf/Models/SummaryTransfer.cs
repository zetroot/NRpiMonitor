using System.Text.Json.Serialization;

namespace NRpiMonitor.Services.iperf.Models;

public class SummaryTransfer
{
    [JsonPropertyName("bits_per_second")]
    public double Bandwidth { get; set; }
    
    [JsonPropertyName("bytes")]
    public long Size { get; set; }
        
    [JsonPropertyName("seconds")]
    public double Time { get; set; }
}
