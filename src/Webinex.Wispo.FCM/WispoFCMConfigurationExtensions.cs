using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM;

public static class WispoFCMConfigurationExtensions
{
    public static IWispoFCMConfiguration AddDevicesDbContext<TDbContext>(this IWispoFCMConfiguration configuration)
        where TDbContext : IWispoFCMDevicesDbContext
    {
        configuration.AddDevicesDbContext(typeof(TDbContext));
        return configuration;
    }
}