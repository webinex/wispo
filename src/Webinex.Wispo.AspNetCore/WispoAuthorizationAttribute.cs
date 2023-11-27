using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Wispo.AspNetCore;

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
internal class WispoAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var setting = context.HttpContext.RequestServices.GetRequiredService<IWispoAspNetCoreSettings>();
        
        if (setting.Policy == null)
            return;
        
        if (await IsAuthorizedAsync(context, setting))
            return;

        context.Result = new ForbidResult();
    }

    private async Task<bool> IsAuthorizedAsync(AuthorizationFilterContext context, IWispoAspNetCoreSettings setting)
    {
        
        var authenticationService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        var authenticationResult = await authenticationService.AuthenticateAsync(context.HttpContext, setting.Schema);
        if (!authenticationResult.Succeeded)
            return false;

        var authorizationResult =
            await authorizationService.AuthorizeAsync(authenticationResult.Principal!, setting.Policy!);

        return authorizationResult.Succeeded;
    }
}