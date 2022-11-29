using System.Diagnostics;
using System.Text.Json;
using NRpiMonitor.Database.Repositories;
using NRpiMonitor.Services.Models.Speedtest;

namespace NRpiMonitor.Services;

public class SpeedtestService
{
    private readonly SpeedTestRepository _repo;

    public SpeedtestService(SpeedTestRepository repo)
    {
        this._repo = repo;
    }

    public async Task RunSpeedtest()
    {
        var proc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "speedtest",
                Arguments = "--accept-license --format=json",
                RedirectStandardOutput = true
            }
        };
        proc.Start();
        var output = await proc.StandardOutput.ReadToEndAsync();
        var result = JsonSerializer.Deserialize<SpeedtestResultJson>(output);
        await _repo.AddResult(result);
    }
    
    public Task<SpeedtestResult> GetLastState() => _repo.GetLastState();
    public Task<List<SpeedtestResult>> GetResults(DateTime notBefore, DateTime notAfter) =>
        _repo.GetAllResults(notBefore, notAfter);
}
