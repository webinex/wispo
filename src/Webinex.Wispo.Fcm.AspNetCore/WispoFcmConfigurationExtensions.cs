using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.Fcm.AspNetCore;

public static class WispoFcmConfigurationExtensions
{
    public static IWispoFcmConfiguration AddDevicesApiServices(this IWispoFcmConfiguration @this)
    {
        @this.Services
            .TryAddSingleton<IWispoFcmDeviceDtoMapper, DefaultWispoFcmDeviceDtoMapper>();

        return @this;
    }

    public static IWispoFcmConfiguration AddWebConfigServices(
        this IWispoFcmConfiguration @this,
        Action<WispoFcmWebConfig> configure)
    {
        var options = new WispoFcmWebConfig();
        configure(options);
        options.Validate();

        @this.Services.TryAddSingleton(options);

        return @this;
    }
}

public class WispoFcmWebConfig
{
    public string JsonCredentialData { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
    public string AuthDomain { get; init; } = null!;
    public string ProjectId { get; init; } = null!;
    public string StorageBucket { get; init; } = null!;
    public string MessagingSenderId { get; init; } = null!;
    public string AppId { get; init; } = null!;
    public string VapidKey { get; init; } = null!;
    public string? MeasurementId { get; init; }

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(JsonCredentialData))
            throw new ArgumentException("JsonCredentialData is required");
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new ArgumentException("ApiKey is required");
        if (string.IsNullOrWhiteSpace(AuthDomain))
            throw new ArgumentException("AuthDomain is required");
        if (string.IsNullOrWhiteSpace(ProjectId))
            throw new ArgumentException("ProjectId is required");
        if (string.IsNullOrWhiteSpace(StorageBucket))
            throw new ArgumentException("StorageBucket is required");
        if (string.IsNullOrWhiteSpace(MessagingSenderId))
            throw new ArgumentException("MessagingSenderId is required");
        if (string.IsNullOrWhiteSpace(AppId))
            throw new ArgumentException("AppId is required");
        if (string.IsNullOrWhiteSpace(VapidKey))
            throw new ArgumentException("VapidKey is required");
    }
}