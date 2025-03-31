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
}