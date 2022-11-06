using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NRpiMonitor.Database
{
    [ExcludeFromCodeCoverage]
    internal class DatabaseContextDesignFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlite("Data source=:inmemory");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
