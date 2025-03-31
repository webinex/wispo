using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Wispo.FCM.Devices;

internal class WispoFCMStaleDevicesCleaningJobOptions
{
    public TimeSpan CleaningInterval { get; }

    public WispoFCMStaleDevicesCleaningJobOptions(TimeSpan cleaningInterval)
    {
        CleaningInterval = cleaningInterval;
    }
}

internal class WispoFCMStaleDevicesCleaningJob : BackgroundService
{
    private readonly PeriodicTimer _timer;

    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public WispoFCMStaleDevicesCleaningJob(
        WispoFCMStaleDevicesCleaningJobOptions options,
        ILogger<WispoFCMStaleDevicesCleaningJob> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _timer = new PeriodicTimer(options.CleaningInterval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stale devices cleaning job is started");

        await DoWorkAsync(stoppingToken);
        
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Trying to remove stale devices");
        await using var scope = _serviceProvider.CreateAsyncScope();
        var deviceService = scope.ServiceProvider.GetRequiredService<IWispoFCMDeviceService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<IWispoFCMDeviceDbContext>();
        var removed = await deviceService.RemoveStaleAsync();

        if (!removed.Any())
        {
            _logger.LogInformation("No stale devices found");
            return;
        }

        await dbContext.SaveChangesAsync(stoppingToken);
        _logger.LogInformation("Removed {Number} stale device(s)", removed.Length);
    }
}