namespace NRpiMonitor.Services;

public record PingCheckResult(string Host, int TotalCount, int SuccessCount, double AvgRtt, double MinRtt, double MaxRtt);