namespace NRpiMonitor.Services;

public record PingCheckResult(DateTime Timestamp, string Host, int TotalCount, int SuccessCount, double AvgRtt, double MinRtt, double MaxRtt);
