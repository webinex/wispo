using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Webinex.Wispo.Fcm.Devices;

namespace Webinex.Wispo.Fcm.AspNetCore;

public interface IWispoFcmDeviceDtoMapper
{
    Task<WispoAddOrUpdateFcmDeviceArgs> Map(WispoFcmRegisterDeviceDto dto);
}

internal class DefaultWispoFcmDeviceDtoMapper : IWispoFcmDeviceDtoMapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DefaultWispoFcmDeviceDtoMapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<WispoAddOrUpdateFcmDeviceArgs> Map(WispoFcmRegisterDeviceDto dto)
    {
        var recipientId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (recipientId == null)
            throw new InvalidOperationException(
                $"RecipientId was not resolved from the HttpContext. " +
                $"Consider using custom implementation of {nameof(IWispoFcmDeviceDtoMapper)}");

        return Task.FromResult(new WispoAddOrUpdateFcmDeviceArgs(dto.Token, recipientId, GetMeta(dto)));
    }

    private static string GetMeta(WispoFcmRegisterDeviceDto dto)
    {
        var meta = JsonSerializer.SerializeToElement(new
        {
            DeviceType = dto.DeviceType ?? throw new ArgumentNullException()
        });
        var metaObject = JsonObject.Create(meta) ?? throw new InvalidOperationException();

        if (dto.Extra == null)
            return metaObject.ToJsonString();

        foreach (var (key, value) in dto.Extra)
            metaObject.Add(key, value);

        return metaObject.ToJsonString();
    }
}