using System.Text;
using TestProject.CurrencyUpdater.Cbr;
using TestProject.CurrencyUpdater.Persistence;
using TestProject.CurrencyUpdater.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestProject.Shared.Persistence;

namespace TestProject.CurrencyUpdater;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((ctx, services) =>
            {
                // Options
                services.Configure<CbrOptions>(ctx.Configuration.GetSection("Cbr"));
                services.Configure<CurrencyUpdaterOptions>(ctx.Configuration.GetSection("CurrencyUpdater"));

                // HttpClient
                services.AddHttpClient("cbr", client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(20);
                });

                // DbContext
                var cs = ctx.Configuration.GetConnectionString("Postgres")
                         ?? ctx.Configuration["POSTGRES_CONNECTION"];

                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException("Connection string not found. Set ConnectionStrings:Postgres or POSTGRES_CONNECTION.");

                services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));

                // Repo + Worker
                services.AddScoped<CurrencyUpsertRepository>();
                services.AddHostedService<CurrencyRatesWorker>();
            })
            .Build();

        await host.RunAsync();
    }
}