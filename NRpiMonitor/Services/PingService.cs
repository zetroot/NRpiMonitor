using System.Net.NetworkInformation;

namespace NRpiMonitor.Services;

public class PingService
{
    public async Task<PingCheckResult> PingHost(string host, int num)
    {
        using var ping = new Ping();
        var success = 0;
        var rtts = new List<long>(num);
        for (int i = 0; i < num; ++i)
        {
            var result = await ping.SendPingAsync(host);
            if (result.Status == IPStatus.Success)
            {
                rtts.Add(result.RoundtripTime);
                success++;
            }
        }

        return new(host, num, success, rtts.Sum(x => (double)x) / success, rtts.Min(x => (double)x), rtts.Max(x => (double)x));
    }
}
