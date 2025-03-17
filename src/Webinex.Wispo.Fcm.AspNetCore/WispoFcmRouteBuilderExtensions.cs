using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Webinex.Wispo.Fcm.Devices;

namespace Webinex.Wispo.Fcm.AspNetCore;

public static class WispoFcmRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapWispoFcmDevicesApi(
        this IEndpointRouteBuilder endpoints,
        Action<RouteHandlerBuilder>? configure = null,
        string route = "/api/wispo/fcm/devices")
    {
        var registerDevice = endpoints.MapPut(
                route,
                async (
                    [FromBody] WispoFcmRegisterDeviceDto dto,
                    [FromServices] IWispoFcmDevicesService fcmDevicesService,
                    [FromServices] IWispoFcmDeviceDtoMapper dtoMapper,
                    [FromServices] IWispoFcmDevicesDbContext dbContext) =>
                {
                    var args = await dtoMapper.Map(dto);
                    await fcmDevicesService.AddOrUpdateAsync(args);
                    await dbContext.SaveChangesAsync();

                    return Results.Ok();
                })
            .WithGroupName("WispoFcmDevices")
            .WithName("RegisterDevice")
            .WithOpenApi();

        configure?.Invoke(registerDevice);

        return endpoints;
    }

    public static IEndpointRouteBuilder MapWispoFcmWebConfigApi(
        this IEndpointRouteBuilder endpoints,
        Action<RouteHandlerBuilder>? configure = null,
        string route = "/api/wispo/fcm/web/config")
    {
        var getWebConfig = endpoints.MapGet(
                route,
                ([FromServices] WispoFcmWebConfig options) => Results.Ok(options))
            .WithGroupName("WispoFcmWeb")
            .WithName("GetConfig")
            .WithOpenApi();

        configure?.Invoke(getWebConfig);

        return endpoints;
    }
}