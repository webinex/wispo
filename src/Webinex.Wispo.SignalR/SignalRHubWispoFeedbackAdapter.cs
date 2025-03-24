using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Webinex.Wispo.AspNetCore;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.SignalR;

internal class SignalRHubWispoFeedbackAdapter<TData, THub> : IWispoFeedbackPort<TData>
    where THub : Hub
{
    private readonly IWispoFeedbackPort<TData> _next;
    private readonly IHubContext<THub> _hubContext;
    private readonly IWispoAspNetCoreMapper<TData> _mapper;
    private readonly ILogger _logger;

    public SignalRHubWispoFeedbackAdapter(
        IWispoFeedbackPort<TData> next,
        IHubContext<THub> hubContext,
        IWispoAspNetCoreMapper<TData> mapper,
        ILogger<SignalRHubWispoFeedbackAdapter<TData, THub>> logger)
    {
        _next = next;
        _hubContext = hubContext;
        _mapper = mapper;
        _logger = logger;
        _logger = logger;
    }

    public async Task SendNewAsync(IEnumerable<Notification<TData>> notifications)
    {
        var array = notifications as Notification<TData>[] ?? notifications.ToArray();

        try
        {
            await SendNewInternalAsync(array);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send SignalR new notification feedback");
        }

        await _next.SendNewAsync(array);
    }

    private async Task SendNewInternalAsync(Notification<TData>[] notifications)
    {
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
        var array = notifications as Notification<TData>[] ?? notifications.ToArray();

        try
        {
            await SendReadInternalAsync(array);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send SignalR read notification feedback");
        }

        await _next.SendNewAsync(array);
    }

    private async Task SendReadInternalAsync(Notification<TData>[] notifications)
    {
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