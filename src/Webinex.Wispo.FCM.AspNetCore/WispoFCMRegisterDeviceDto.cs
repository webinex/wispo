using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Webinex.Wispo.FCM.AspNetCore;

public class WispoFCMRegisterDeviceDto
{
    [Required]
    public string Token { get; init; } = null!;

    [Required]
    public string DeviceType { get; init; } = null!;

    public JsonObject? Extra { get; init; }
}