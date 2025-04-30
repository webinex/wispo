using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webinex.Wispo.FCM.AspNetCore;

public interface IWispoFCMContextResolver
{
    Task<WispoFCMContext> Resolve();
}

public class WispoFCMContext
{
    public required string RecipientId { get; init; }
}

internal class HttpContextResolverWispoFCMContextResolver : IWispoFCMContextResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextResolverWispoFCMContextResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<WispoFCMContext> Resolve()
    {
        var recipientId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (recipientId == null)
            throw new InvalidOperationException($"RecipientId was not resolved from the HttpContext.");

        return Task.FromResult(new WispoFCMContext
        {
            RecipientId = recipientId,
        });
    }
}