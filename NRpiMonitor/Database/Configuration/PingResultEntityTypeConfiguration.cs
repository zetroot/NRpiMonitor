using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NRpiMonitor.Database.Model;

namespace NRpiMonitor.Database.Configuration;

public class PingResultEntityTypeConfiguration : IEntityTypeConfiguration<PingResultDal>
{
    public void Configure(EntityTypeBuilder<PingResultDal> builder)
    {
        builder.ToTable("PingResults");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.Host);
        builder.HasIndex(x => x.Timestamp);
    }
}
