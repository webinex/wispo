using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webinex.Wispo.AspNetCore;

internal class DefaultWispoAspNetCoreContextService : IWispoAspNetCoreContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DefaultWispoAspNetCoreContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Account> GetAsync()
    {
        var id = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
            throw new InvalidOperationException($"Unable to find claim {ClaimTypes.NameIdentifier} in http context user claims.");

        return Task.FromResult(new Account(id));
    }
}