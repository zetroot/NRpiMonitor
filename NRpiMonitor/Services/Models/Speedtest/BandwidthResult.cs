using System.Text.Json.Serialization;

namespace NRpiMonitor.Services.Models.Speedtest;

public class BandwidthResult
{
    [JsonPropertyName("bandwidth")]
    public int Bandwidth { get; set; }
}
