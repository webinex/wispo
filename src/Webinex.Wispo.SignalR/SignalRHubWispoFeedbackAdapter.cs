using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Webinex.Wispo.AspNetCore;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.SignalR;

internal class SignalRHubWispoFeedbackAdapter<TData, THub> : IWispoFeedbackPort<TData>
    where THub : Hub
{
    private readonly IHubContext<THub> _hubContext;
    private readonly IWispoAspNetCoreMapper<TData> _mapper;

    public SignalRHubWispoFeedbackAdapter(IHubContext<THub> hubContext, IWispoAspNetCoreMapper<TData> mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task SendNewAsync(IEnumerable<Notification<TData>> notifications)
    {
        notifications = notifications?.ToArray() ?? throw new ArgumentNullException(nameof(notifications));
        var mapped = await _mapper.MapAsync(notifications.ToArray());
        
        foreach (var group in notifications.GroupBy(x => x.RecipientId).ToArray())
        {
            await _hubContext.Clients
                .Group(group.Key)
                .SendAsync(
                    WispoSignalRMessages.NEW,
                    group.Select(x => mapped.Single(m => m.Id == x.Id)));
        }
    }

    public async Task SendReadAsync(IEnumerable<Notification<TData>> notifications)
    {
        notifications = notifications?.ToArray() ?? throw new ArgumentNullException(nameof(notifications));

        foreach (var group in notifications.GroupBy(x => x.RecipientId).ToArray())
        {
            await _hubContext.Clients
                .Group(group.Key)
                .SendAsync(
                    WispoSignalRMessages.READ,
                    group.Select(x => x.Id));
        }
    }
}