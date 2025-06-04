using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.FCM.AspNetCore;

public static class WispoFCMConfigurationExtensions
{
    /// <summary>
    /// Adds required services for Wispo Devices API 
    /// </summary>
    public static IWispoFCMConfiguration AddDeviceApiCore(this IWispoFCMConfiguration @this)
    {
        @this.Services.TryAddScoped<IWispoFCMDevicesApi, WispoFCMDevicesApi>();
        @this.Services.TryAddSingleton<IWispoFCMContextResolver, HttpContextResolverWispoFCMContextResolver>();
        @this.Services.TryAddSingleton<IWispoFCMDeviceDtoMapper, DefaultWispoFCMDeviceDtoMapper>();

        return @this;
    }

    /// <summary>
    /// Adds required services for Wispo Web Config API 
    /// </summary>
    public static IWispoFCMConfiguration AddWebConfigApiCore(
        this IWispoFCMConfiguration @this,
        WispoFCMWebSettings settings)
    {
        @this.Services.TryAddSingleton(settings);
        @this.Services.TryAddSingleton<IWispoFCMWebConfigApi, WispoFCMWebConfigApi>();

        return @this;
    }
}

public class WispoFCMWebSettings
{
    public string ApiKey { get; init; }
    public string AuthDomain { get; init; }
    public string ProjectId { get; init; }
    public string StorageBucket { get; init; }
    public string MessagingSenderId { get; init; }
    public string AppId { get; init; }
    public string VapidKey { get; init; }
    public string? MeasurementId { get; init; }

    public WispoFCMWebSettings(
        string apiKey,
        string authDomain,
        string projectId,
        string storageBucket,
        string messagingSenderId, 
        string appId,
        string vapidKey,
        string? measurementId)
    {
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
        
        ApiKey = apiKey;
        AuthDomain = authDomain;
        ProjectId = projectId;
        StorageBucket = storageBucket;
        MessagingSenderId = messagingSenderId;
        AppId = appId;
        VapidKey = vapidKey;
        MeasurementId = measurementId;
    }
}