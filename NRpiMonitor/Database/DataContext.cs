using Microsoft.EntityFrameworkCore;
using NRpiMonitor.Database.Model;

namespace NRpiMonitor.Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions opts) : base(opts)
    {
        this.Database.Migrate();
    }
    
    public DbSet<PingResultDal> Pings => Set<PingResultDal>();
    public DbSet<SpeedtestResultDal> SpeedTests => Set<SpeedtestResultDal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
