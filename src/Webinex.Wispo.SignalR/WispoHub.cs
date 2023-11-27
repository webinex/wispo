using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Wispo.AspNetCore;

namespace Webinex.Wispo.SignalR;

public class WispoHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var contextService = Context.GetHttpContext().RequestServices
            .GetRequiredService<IWispoAspNetCoreContextService>();
        var context = await contextService.GetAsync();
        foreach (var id in context.All)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToLowerInvariant());
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var contextService = Context.GetHttpContext().RequestServices
            .GetRequiredService<IWispoAspNetCoreContextService>();
        var context = await contextService.GetAsync();
        foreach (var id in context.All)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToLowerInvariant());
        }
        await base.OnDisconnectedAsync(exception);
    }
}