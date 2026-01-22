using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestProject.Shared;
using TestProject.Shared.Persistence; // AppDbContext

namespace TestProject.Migrations
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILoggerFactory>()
                                      .CreateLogger("Migrator");

            try
            {
                logger.LogInformation("Starting database migration...");

                using var scope = host.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await WaitForDatabaseAsync(db, logger);

                var pending = await db.Database.GetPendingMigrationsAsync();
                var enumerable = pending as string[] ?? pending.ToArray();
                if (enumerable.Length != 0)
                {
                    logger.LogInformation("Pending migrations: {Count}", enumerable.Length);
                    foreach (var migration in enumerable)
                        logger.LogInformation(" - {Migration}", migration);

                    await db.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("No pending migrations. Database is up to date.");
                }

                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration failed.");
                return 1;
            }
            finally
            {
                await host.StopAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = ResolveConnectionString(context.Configuration);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseNpgsql(connectionString, npgsql =>
                        {
                            // Important: keep migrations in this assembly
                            npgsql.MigrationsAssembly(typeof(Program).Assembly.FullName);
                        });

                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    });
                });
        }

        private static string ResolveConnectionString(IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("Postgres");
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            cs = configuration["POSTGRES_CONNECTION"];
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            throw new InvalidOperationException(
                "Postgres connection string not found. " +
                "Set ConnectionStrings:Postgres or POSTGRES_CONNECTION.");
        }

        private static async Task WaitForDatabaseAsync(
            AppDbContext db,
            ILogger logger,
            int maxAttempts = 30,
            int delaySeconds = 2)
        {
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    logger.LogInformation(
                        "Checking database connectivity (attempt {Attempt}/{Max})...",
                        attempt, maxAttempts);

                    if (await db.Database.CanConnectAsync())
                    {
                        logger.LogInformation("Database connection OK.");
                        return;
                    }

                    logger.LogWarning("Database not ready yet.");
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    logger.LogWarning(ex, "Database not ready yet (transient error).");
                }

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }

            throw new TimeoutException(
                $"Database is not reachable after {maxAttempts} attempts.");
        }

        private static bool IsTransient(Exception ex)
        {
            return ex is SocketException
                || ex is TimeoutException
                || ex.InnerException is SocketException
                || ex.InnerException is TimeoutException;
        }
    }
}
