using ADSCrossPlatform.Code.Models;
using Microsoft.EntityFrameworkCore;

namespace ADSCrossPlatform.Code.Context
{
    namespace ADS.PostgreSQL
    {
        internal class PostgreSQLContext : DbContext
        {
            public DbSet<AddressDBModel> Addresses { get; set; } = null!;

            public PostgreSQLContext()
            {
                Database.EnsureCreated();   // создаем базу данных при первом обращении
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {                
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=adsV2db;Username=postgres;Password=Inter101!");
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }
    }
}
