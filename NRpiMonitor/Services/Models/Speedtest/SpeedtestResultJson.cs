using System.Text.Json.Serialization;

namespace NRpiMonitor.Services.Models.Speedtest;

public class SpeedtestResultJson
{
    [JsonPropertyName("download")]
    public BandwidthResult Download { get; set; }
    
    [JsonPropertyName("upload")]
    public BandwidthResult Upload { get; set; }
}
