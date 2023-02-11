namespace NRpiMonitor.Services.iperf.Models;

public class IperfSettings
{
    public TimeSpan Cooldown { get; set; }
    public IperfHost[] Hosts { get; set; }
}

public class IperfHost
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string RsaKeyfile { get; set; }
}
