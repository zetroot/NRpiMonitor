using System.Text.Json.Serialization;

namespace NRpiMonitor.Services.iperf.Models;

public class SummaryNode
{
    [JsonPropertyName("sum_sent")]
    public SummaryTransfer? Sent { get; set; }
    
    [JsonPropertyName("sum_received")]
    public SummaryTransfer? Received { get; set; }
}