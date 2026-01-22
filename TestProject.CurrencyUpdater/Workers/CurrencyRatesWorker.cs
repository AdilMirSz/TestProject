using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestProject.CurrencyUpdater.Cbr;
using TestProject.CurrencyUpdater.Persistence;
using Microsoft.Extensions.Options;

namespace TestProject.CurrencyUpdater.Workers;

public sealed class CurrencyRatesWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CurrencyRatesWorker> _logger;
    private readonly string _url;
    private readonly TimeSpan _interval;

    public CurrencyRatesWorker(
        IServiceProvider services,
        IHttpClientFactory httpClientFactory,
        IOptions<CbrOptions> cbrOptions,
        IOptions<CurrencyUpdaterOptions> updaterOptions,
        ILogger<CurrencyRatesWorker> logger)
    {
        _services = services;
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        _url = cbrOptions.Value.Url;
        _interval = TimeSpan.FromMinutes(Math.Max(1, updaterOptions.Value.IntervalMinutes));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Currency updater started. Interval: {Interval}. Url: {Url}", _interval, _url);

        // Можно сделать "сразу обновить при старте"
        await TryUpdateOnce(stoppingToken);

        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await TryUpdateOnce(stoppingToken);
        }
    }

    private async Task TryUpdateOnce(CancellationToken ct)
    {
        try
        {
            var http = _httpClientFactory.CreateClient("cbr");
            var xml = await http.GetStringAsync(_url, ct);

            var parsed = CbrXmlParser.Parse(xml);
            var updatedAtUtc = DateTime.UtcNow;

            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<CurrencyUpsertRepository>();

            await repo.UpsertManyAsync(parsed.Select(x => (x.Name, x.Rate)).ToList(), updatedAtUtc, ct);

            _logger.LogInformation("Updated {Count} currencies at {TimeUtc}", parsed.Count, updatedAtUtc);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // нормальное завершение
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update currencies.");
        }
    }
}
