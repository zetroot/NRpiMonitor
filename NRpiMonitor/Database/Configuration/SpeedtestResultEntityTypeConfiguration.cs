using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NRpiMonitor.Database.Model;

namespace NRpiMonitor.Database.Configuration;

public class SpeedtestResultEntityTypeConfiguration : IEntityTypeConfiguration<SpeedtestResultDal>
{
    public void Configure(EntityTypeBuilder<SpeedtestResultDal> builder)
    {
        builder.ToTable("SpeedtestResults");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Timestamp);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}
