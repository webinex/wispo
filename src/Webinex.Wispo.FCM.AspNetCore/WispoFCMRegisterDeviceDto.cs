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
        Token = token;
        DeviceType = deviceType;
        Extra = extra;
    }
}