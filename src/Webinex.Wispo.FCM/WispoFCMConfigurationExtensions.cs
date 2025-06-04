using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM;

public static class WispoFCMConfigurationExtensions
{
    public static IWispoFCMConfiguration UseDevicesDbContext<TDbContext>(this IWispoFCMConfiguration configuration)
        where TDbContext : IWispoFCMDeviceDbContext
    {
        configuration.UseDevicesDbContext(typeof(TDbContext));
        return configuration;
    }
    
    
    /// <summary>
    /// Adds a job that cleans up the database from staled devices. By default runs every 12 hours.
    /// </summary>
    public static IWispoFCMConfiguration AddStaleDevicesCleaningJob(
        this IWispoFCMConfiguration @this,
        WispoFCMStaleDevicesCleaningJobOptions? options = null)
    {
        @this.Services.TryAddSingleton(options ?? new WispoFCMStaleDevicesCleaningJobOptions());
        @this.Services.AddHostedService<WispoFCMStaleDevicesCleaningJob>();

        return @this;
    }
}