using System.Threading.Tasks;

namespace Webinex.Wispo.FCM.AspNetCore;

public interface IWispoFCMWebConfigApi
{
    Task<WispoFCMWebSettings> GetConfig();
}

internal class WispoFCMWebConfigApi : IWispoFCMWebConfigApi
{
    private readonly WispoFCMWebSettings _options;

    public WispoFCMWebConfigApi(WispoFCMWebSettings options)
    {
        _options = options;
    }

    public Task<WispoFCMWebSettings> GetConfig()
    {
        return Task.FromResult(_options);
    }
}