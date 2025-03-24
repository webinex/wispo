using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.FCM.AspNetCore;

public static class WispoFCMConfigurationExtensions
{
    /// <summary>
    /// Adds required services for Wispo Devices API 
    /// </summary>
    public static IWispoFCMConfiguration AddDevicesApiServices(this IWispoFCMConfiguration @this)
    {
        @this.Services.TryAddSingleton<IWispoFCMRecipientIdResolver, HttpContextWispoFCMRecipientIdResolver>();
        @this.Services.TryAddSingleton<IWispoFCMDeviceDtoMapper, DefaultWispoFCMDeviceDtoMapper>();

        return @this;
    }

    /// <summary>
    /// Adds required services for Wispo Web Config API 
    /// </summary>
    public static IWispoFCMConfiguration AddWebConfigApiServices(
        this IWispoFCMConfiguration @this,
        Action<WispoFCMWebConfig> configure)
    {
        var options = new WispoFCMWebConfig();
        configure(options);
        options.Validate();

        @this.Services.TryAddSingleton(options);

        return @this;
    }

    /// <summary>
    /// Adds a job that cleans up the database from staled devices. By default runs every 12 hours.
    /// </summary>
    public static IWispoFCMConfiguration AddAutoCleanJob(this IWispoFCMConfiguration @this, TimeSpan? cleanEvery = null)
    {
        cleanEvery ??= TimeSpan.FromHours(12);
        // TODO: implement
        // @this.Services.AddHostedService()
        return @this;
    }
}

public class WispoFCMWebConfig
{
    public string ApiKey { get; set; } = null!;
    public string AuthDomain { get; set; } = null!;
    public string ProjectId { get; set; } = null!;
    public string StorageBucket { get; set; } = null!;
    public string MessagingSenderId { get; set; } = null!;
    public string AppId { get; set; } = null!;
    public string VapidKey { get; set; } = null!;
    public string? MeasurementId { get; set; }

    internal void Validate()
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
    }
}