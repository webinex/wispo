using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webinex.Wispo.FCM.AspNetCore;

internal interface IWispoFCMContext
{
    Task<WispoFCMContextData> Resolve();
}

public class WispoFCMContextData
{
    public required string RecipientId { get; init; }
}

internal class HttpContextWispoFCMContext : IWispoFCMContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextWispoFCMContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<WispoFCMContextData> Resolve()
    {
        var recipientId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (recipientId == null)
            throw new InvalidOperationException($"RecipientId was not resolved from the HttpContext.");

        return Task.FromResult(new WispoFCMContextData
        {
            RecipientId = recipientId,
        });
    }
}