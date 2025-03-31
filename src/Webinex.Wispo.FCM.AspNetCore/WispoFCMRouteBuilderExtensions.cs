using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Webinex.Wispo.FCM.Devices;

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
                    [FromServices] IWispoFCMDeviceService devicesService,
                    [FromServices] IWispoFCMDeviceDtoMapper dtoMapper,
                    [FromServices] IWispoFCMDeviceDbContext dbContext) =>
                {
                    var args = await dtoMapper.MapAsync(dto);
                    await devicesService.AddOrUpdateAsync(args);
                    await dbContext.SaveChangesAsync(CancellationToken.None);

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
                ([FromServices] WispoFCMWebSettings options) => Results.Ok(options))
            .WithTags("WispoFCMWeb")
            .WithName("GetConfig")
            .WithOpenApi();

        configure?.Invoke(getWebConfig);

        return endpoints;
    }
}