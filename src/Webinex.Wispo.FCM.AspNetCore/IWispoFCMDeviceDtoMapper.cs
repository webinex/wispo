using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM.AspNetCore;

public interface IWispoFCMDeviceDtoMapper
{
    Task<WispoAddOrUpdateFCMDeviceArgs> Map(WispoFCMRegisterDeviceDto dto);
}

internal class DefaultWispoFCMDeviceDtoMapper : IWispoFCMDeviceDtoMapper
{
    private readonly IWispoFCMRecipientIdResolver _recipientIdResolver;

    public DefaultWispoFCMDeviceDtoMapper(IWispoFCMRecipientIdResolver recipientIdResolver)
    {
        _recipientIdResolver = recipientIdResolver;
    }

    public async Task<WispoAddOrUpdateFCMDeviceArgs> Map(WispoFCMRegisterDeviceDto dto)
    {
        var recipientId = await _recipientIdResolver.Resolve();

        return new WispoAddOrUpdateFCMDeviceArgs(dto.Token, recipientId, GetMeta(dto));
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