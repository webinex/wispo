using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.Fcm.AspNetCore;

public static class WispoFcmConfigurationExtensions
{
    /// <summary>
    /// Adds required services for Wispo Devices API 
    /// </summary>
    public static IWispoFcmConfiguration AddDevicesApiServices(this IWispoFcmConfiguration @this)
    {
        @this.Services
            .TryAddSingleton<IWispoFcmDeviceDtoMapper, DefaultWispoFcmDeviceDtoMapper>();

        return @this;
    }

    /// <summary>
    /// Adds required services for Wispo Web Config API 
    /// </summary>
    public static IWispoFcmConfiguration AddWebConfigApiServices(
        this IWispoFcmConfiguration @this,
        Action<WispoFcmWebConfig> configure)
    {
        var options = new WispoFcmWebConfig();
        configure(options);
        options.Validate();

        @this.Services.TryAddSingleton(options);

        return @this;
    }

    /// <summary>
    /// Adds a job that cleans up the database from staled devices. By default runs every 12 hours.
    /// </summary>
    public static IWispoFcmConfiguration AddAutoCleanJob(this IWispoFcmConfiguration @this, TimeSpan? cleanEvery = null)
    {
        cleanEvery ??= TimeSpan.FromHours(12);
        // TODO: implement
        // @this.Services.AddHostedService()
        return @this;
    }
}

public class WispoFcmWebConfig
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