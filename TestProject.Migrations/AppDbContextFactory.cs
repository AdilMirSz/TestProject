using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TestProject.Shared;
using TestProject.Shared.Persistence; // AppDbContext

namespace TestProject.Migrations
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = configuration.GetConnectionString("Postgres")
                     ?? configuration["POSTGRES_CONNECTION"];

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException(
                    "Connection string not found. Set ConnectionStrings:Postgres or POSTGRES_CONNECTION.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(cs, npgsql =>
            {
                // Critical: migrations are in THIS assembly (TestProject.Migrations)
                npgsql.MigrationsAssembly(typeof(AppDbContextFactory).Assembly.FullName);
            });

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}