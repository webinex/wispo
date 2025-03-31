using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Webinex.Wispo.FCM.AspNetCore;

public class WispoFCMRegisterDeviceDto
{
    [Required]
    public string Token { get; init; }

    [Required]
    public string DeviceType { get; init; }

    public JsonObject? Extra { get; init; }

    public WispoFCMRegisterDeviceDto(string token, string deviceType, JsonObject? extra)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        DeviceType = deviceType ?? throw new ArgumentNullException(nameof(deviceType));
        Extra = extra;
    }
}