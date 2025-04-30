using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM;

public static class WispoConfigurationExtensions
{
    /// <summary>
    ///     Adds Firebase Cloud Messaging (FCM) feedback to Wispo
    /// </summary>
    /// <param name="configuration"><see cref="IWispoConfiguration"/></param>
    /// <param name="configure"><see cref="Action{IWispoFCMConfiguration}"/></param>
    /// <returns><see cref="IWispoConfiguration"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IWispoConfiguration AddFCMFeedback(
        this IWispoConfiguration configuration,
        Action<IWispoFCMConfiguration> configure)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        configure = configure ?? throw new ArgumentNullException(nameof(configure));

        var cfg = new WispoFCMConfiguration(configuration);
        configure(cfg);
        cfg.Validate();

        var services = configuration.Services;

        configuration
            .AppendFeedbackAdapter(typeof(WispoFCMFeedbackAdapter<>).MakeGenericType(configuration.DataType));

        services.TryAddSingleton(
            typeof(IWispoFCMMessageMapper<>).MakeGenericType(configuration.DataType),
            typeof(DefaultWispoFCMMessageMapper<>).MakeGenericType(configuration.DataType));

        services.TryAddScoped<IWispoFCMDeviceService, WispoFCMDeviceService>();

        services.TryAddSingleton<IWispoFCMSender, WispoFCMSender>();
        services.TryAddSingleton(new WispoFCMOptions { JsonCredentialData = cfg.JsonCredentialData, });
        services.TryAddSingleton(new WispoFCMDevicesOptions { KeepUnusedFor = cfg.KeepUnusedFor, });

        return configuration;
    }
}

internal class WispoFCMOptions
{
    public string JsonCredentialData { get; init; } = null!;
}

internal class WispoFCMDevicesOptions
{
    public TimeSpan KeepUnusedFor { get; init; }
}

public interface IWispoFCMConfiguration
{
    /// <summary>
    /// Service collection. Can be used in child packages.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Sets FCM JSON credential data. This data will be used to communicate with FCM
    /// </summary>
    IWispoFCMConfiguration UseFCMJsonCredentialData(string data);

    /// <summary>
    /// Sets the period of inactivity after which devices will be considered stale. Must be positive and greater than 0.
    /// </summary>
    IWispoFCMConfiguration UseDevicesStalePeriod(TimeSpan keepUnusedFor);

    /// <summary>
    /// Sets DbContext type to be used for storing FCM device tokens.
    /// </summary>
    IWispoFCMConfiguration UseDevicesDbContext(Type dbContext);

    IWispoFCMConfiguration UseFCMMessageMapper(
        IReadOnlyDictionary<string, WispoFCMDictMappedMessage> dict,
        Func<string, WispoFCMDictMappedMessage> fallback);
}

internal class WispoFCMConfiguration : IWispoFCMConfiguration
{
    public IServiceCollection Services { get; }
    public Type DataType { get; }
    public string? JsonCredentialData { get; private set; }
    public TimeSpan KeepUnusedFor { get; private set; } = TimeSpan.FromDays(30);

    public WispoFCMConfiguration(IWispoConfiguration configuration)
    {
        DataType = configuration.DataType;
        Services = configuration.Services;
    }

    [MemberNotNull(nameof(JsonCredentialData))]
    internal void Validate()
    {
        if (JsonCredentialData == null)
            throw new InvalidOperationException("FCMJsonCredentialData must be provided");
    }


    public IWispoFCMConfiguration UseFCMJsonCredentialData(string data)
    {
        JsonCredentialData = data ?? throw new ArgumentNullException(nameof(data));
        return this;
    }

    public IWispoFCMConfiguration UseDevicesStalePeriod(TimeSpan keepUnusedFor)
    {
        if (keepUnusedFor <= TimeSpan.Zero)
            throw new ArgumentException("Stale period must be positive and greater than 0");

        this.KeepUnusedFor = keepUnusedFor;
        return this;
    }

    public IWispoFCMConfiguration UseDevicesDbContext(Type dbContext)
    {
        if (!dbContext.IsAssignableTo(typeof(IWispoFCMDeviceDbContext)))
            throw new ArgumentException($"dbContext must implements {nameof(IWispoFCMDeviceDbContext)}");

        Services.TryAddScoped(typeof(IWispoFCMDeviceDbContext), dbContext);
        return this;
    }

    public IWispoFCMConfiguration UseFCMMessageMapper(
        IReadOnlyDictionary<string, WispoFCMDictMappedMessage> dict,
        Func<string, WispoFCMDictMappedMessage> fallback)
    {
        var serviceType = typeof(IWispoFCMMessageMapper<>).MakeGenericType(DataType);
        var implementationType = typeof(DictionaryWispoFCMMessageMapper<>).MakeGenericType(DataType);

        var implementation = Activator.CreateInstance(implementationType, dict, fallback) ??
                       throw new InvalidOperationException();
        Services.TryAddSingleton(serviceType, _ => implementation);

        return this;
    }
}