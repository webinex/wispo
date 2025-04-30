using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Webinex.Wispo.FCM.AspNetCore;

public static class WispoFCMRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapWispoFCMDevicesApi(
        this IEndpointRouteBuilder endpoints,
        Action<RouteHandlerBuilder>? configure = null,
        string route = "/api/wispo/fcm/devices")
    {
        var registerDevice = endpoints.MapPut(
                route,
                async (
                    [FromBody] WispoFCMRegisterDeviceDto dto,
                    [FromServices] IWispoFCMDevicesApi devicesApi) =>
                {
                    await devicesApi.RegisterDevice(dto);
                    return Results.Ok();
                })
            .WithTags("WispoFCMDevices")
            .WithName("RegisterDevice")
            .WithOpenApi();

        configure?.Invoke(registerDevice);

        return endpoints;
    }

    public static IEndpointRouteBuilder MapWispoFCMWebConfigApi(
        this IEndpointRouteBuilder endpoints,
        Action<RouteHandlerBuilder>? configure = null,
        string route = "/api/wispo/fcm/web-config")
    {
        var getWebConfig = endpoints.MapGet(
                route,
                async ([FromServices] IWispoFCMWebConfigApi api) => Results.Ok(await api.GetConfig()))
            .WithTags("WispoFCMWeb")
            .WithName("GetConfig")
            .WithOpenApi();

        configure?.Invoke(getWebConfig);

        return endpoints;
    }
}