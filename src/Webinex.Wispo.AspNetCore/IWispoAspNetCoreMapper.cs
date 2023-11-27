using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webinex.Wispo.AspNetCore;

public interface IWispoAspNetCoreMapper<TData>
{
    Task<IReadOnlyCollection<INotificationBase>> MapAsync(IReadOnlyCollection<Notification<TData>> notifications);
}

internal class DefaultWispoAspNetCoreMapper<TData> : IWispoAspNetCoreMapper<TData>
{
    public Task<IReadOnlyCollection<INotificationBase>> MapAsync(IReadOnlyCollection<Notification<TData>> notifications)
    {
        return Task.FromResult<IReadOnlyCollection<INotificationBase>>(notifications);
    }
}