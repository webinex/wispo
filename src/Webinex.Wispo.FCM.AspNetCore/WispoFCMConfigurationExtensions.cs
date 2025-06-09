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
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey is required");
        if (string.IsNullOrWhiteSpace(authDomain))
            throw new ArgumentException("authDomain is required");
        if (string.IsNullOrWhiteSpace(projectId))
            throw new ArgumentException("projectId is required");
        if (string.IsNullOrWhiteSpace(storageBucket))
            throw new ArgumentException("storageBucket is required");
        if (string.IsNullOrWhiteSpace(messagingSenderId))
            throw new ArgumentException("messagingSenderId is required");
        if (string.IsNullOrWhiteSpace(appId))
            throw new ArgumentException("appId is required");
        if (string.IsNullOrWhiteSpace(vapidKey))
            throw new ArgumentException("vapidKey is required");
        
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