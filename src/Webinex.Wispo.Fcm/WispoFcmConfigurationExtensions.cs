using Webinex.Wispo.Fcm.Devices;

namespace Webinex.Wispo.Fcm;

public static class WispoFcmConfigurationExtensions
{
    public static IWispoFcmConfiguration AddDevicesDbContext<TDbContext>(this IWispoFcmConfiguration configuration)
        where TDbContext : IWispoFcmDevicesDbContext
    {
        configuration.AddDevicesDbContext(typeof(TDbContext));
        return configuration;
    }
}