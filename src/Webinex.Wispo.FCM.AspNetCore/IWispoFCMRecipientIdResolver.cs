using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webinex.Wispo.FCM.AspNetCore;

internal interface IWispoFCMRecipientIdResolver
{
    Task<string> Resolve();
}

internal class HttpContextWispoFCMRecipientIdResolver : IWispoFCMRecipientIdResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextWispoFCMRecipientIdResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> Resolve()
    {
        var recipientId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (recipientId == null)
            throw new InvalidOperationException($"RecipientId was not resolved from the HttpContext.");

        return Task.FromResult(recipientId);
    }
}