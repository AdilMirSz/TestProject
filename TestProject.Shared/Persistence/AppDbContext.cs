using Microsoft.EntityFrameworkCore;
using TestProject.Shared.Persistence.Configuration;
using TestProject.Shared.Persistence.Entities;

namespace TestProject.Shared.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserRow> Users => Set<UserRow>();
        public DbSet<CurrencyRow> Currencies => Set<CurrencyRow>();
        public DbSet<UserFavoriteCurrencyRow> UserFavoriteCurrencies => Set<UserFavoriteCurrencyRow>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new UserFavoriteCurrencyConfiguration());
        }
    }
}