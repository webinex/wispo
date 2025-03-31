using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.FCM.AspNetCore;

public static class WispoFCMConfigurationExtensions
{
    /// <summary>
    /// Adds required services for Wispo Devices API 
    /// </summary>
    public static IWispoFCMConfiguration AddDeviceApiCore(this IWispoFCMConfiguration @this)
    {
        @this.Services.TryAddSingleton<IWispoFCMContext, HttpContextWispoFCMContext>();
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
        settings.Validate();

        @this.Services.TryAddSingleton(settings);

        return @this;
    }

    /// <summary>
    /// Adds a job that cleans up the database from staled devices. By default runs every 12 hours.
    /// </summary>
    public static IWispoFCMConfiguration AddStaleDevicesCleaningJob(
        this IWispoFCMConfiguration @this,
        TimeSpan? cleaningInterval = null)
    {
        cleaningInterval ??= TimeSpan.FromHours(12);
        @this.Services
            .AddSingleton(new WispoFCMStaleDevicesCleaningJobOptions(cleaningInterval.Value))
            .AddHostedService<WispoFCMStaleDevicesCleaningJob>();

        return @this;
    }
}

public class WispoFCMWebSettings
{
    public required string ApiKey { get; init; }
    public required string AuthDomain { get; init; }
    public required string ProjectId { get; init; }
    public required string StorageBucket { get; init; }
    public required string MessagingSenderId { get; init; }
    public required string AppId { get; init; }
    public required string VapidKey { get; init; }
    public string? MeasurementId { get; init; }

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