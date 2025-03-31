using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM.AspNetCore;

public interface IWispoFCMDeviceDtoMapper
{
    Task<FCMDeviceRawValue> MapAsync(WispoFCMRegisterDeviceDto dto);
}

internal class DefaultWispoFCMDeviceDtoMapper : IWispoFCMDeviceDtoMapper
{
    private readonly IWispoFCMContext _context;

    public DefaultWispoFCMDeviceDtoMapper(IWispoFCMContext context)
    {
        _context = context;
    }

    public async Task<FCMDeviceRawValue> MapAsync(WispoFCMRegisterDeviceDto dto)
    {
        var context = await _context.Resolve();

        return new FCMDeviceRawValue(dto.Token, context.RecipientId, GetMeta(dto));
    }

    private static string GetMeta(WispoFCMRegisterDeviceDto dto)
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