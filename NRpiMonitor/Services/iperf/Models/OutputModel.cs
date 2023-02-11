using System.Text.Json.Serialization;

namespace NRpiMonitor.Services.iperf.Models;

public class OutputModelRoot
{
    [JsonPropertyName("end")]
    public SummaryNode? Summary { get; set; }
}
