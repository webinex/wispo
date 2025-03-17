using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Webinex.Wispo.Fcm.AspNetCore;

public class WispoFcmRegisterDeviceDto
{
    [Required]
    public string Token { get; init; } = null!;

    [Required]
    public string DeviceType { get; init; } = null!;

    public JsonObject? Extra { get; init; }
}