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
            typeof(IWispoFCMMessagesMapper<>).MakeGenericType(configuration.DataType),
            typeof(DefaultWispoFCMMessagesMapper<>).MakeGenericType(configuration.DataType));

        services.TryAddScoped<IWispoFCMDevicesService, WispoFCMDevicesService>();

        services.TryAddSingleton<IWispoFCMSender, WispoFCMSender>();
        services.TryAddSingleton(new WispoFCMOptions { FCMJsonCredentialData = cfg.FCMJsonCredentialData, });
        services.TryAddSingleton(new WispoFCMDevicesOptions { ConsiderStaleAfter = cfg.ConsiderStaleAfter, });

        return configuration;
    }
}

internal class WispoFCMOptions
{
    public string FCMJsonCredentialData { get; init; } = null!;
}

internal class WispoFCMDevicesOptions
{
    public TimeSpan ConsiderStaleAfter { get; init; }
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
    IWispoFCMConfiguration UseDevicesStalePeriod(TimeSpan period);

    /// <summary>
    /// Sets DbContext type to be used for storing FCM device tokens.
    /// </summary>
    IWispoFCMConfiguration AddDevicesDbContext(Type dbContext);

    IWispoFCMConfiguration UseDictionaryWispoFCMMessagesMapper(
        IReadOnlyDictionary<string, WispoFCMDictMappedMessage> dict,
        Func<string, WispoFCMDictMappedMessage> fallback);
}

internal class WispoFCMConfiguration : IWispoFCMConfiguration
{
    public IServiceCollection Services { get; }
    public Type DataType { get; }
    public string? FCMJsonCredentialData { get; private set; }
    public TimeSpan ConsiderStaleAfter { get; private set; } = TimeSpan.FromDays(30);

    public WispoFCMConfiguration(IWispoConfiguration configuration)
    {
        DataType = configuration.DataType;
        Services = configuration.Services;
    }

    [MemberNotNull(nameof(FCMJsonCredentialData))]
    public void Validate()
    {
        if (FCMJsonCredentialData == null)
            throw new InvalidOperationException("FCMJsonCredentialData must be provided");
    }


    public IWispoFCMConfiguration UseFCMJsonCredentialData(string data)
    {
        FCMJsonCredentialData = data ?? throw new ArgumentNullException(nameof(data));
        return this;
    }

    public IWispoFCMConfiguration UseDevicesStalePeriod(TimeSpan period)
    {
        if (period <= TimeSpan.Zero)
            throw new ArgumentException("Stale period must be positive and greater than 0");

        ConsiderStaleAfter = period;
        return this;
    }

    public IWispoFCMConfiguration AddDevicesDbContext(Type dbContext)
    {
        if (!dbContext.IsAssignableTo(typeof(IWispoFCMDevicesDbContext)))
            throw new ArgumentException($"dbContext must implements {nameof(IWispoFCMDevicesDbContext)}");

        Services.TryAddScoped(typeof(IWispoFCMDevicesDbContext), dbContext);
        return this;
    }

    public IWispoFCMConfiguration UseDictionaryWispoFCMMessagesMapper(
        IReadOnlyDictionary<string, WispoFCMDictMappedMessage> dict,
        Func<string, WispoFCMDictMappedMessage> fallback)
    {
        var serviceType = typeof(IWispoFCMMessagesMapper<>).MakeGenericType(DataType);
        var implementationType = typeof(DictionaryWispoFCMMessagesMapper<>).MakeGenericType(DataType);
        
        Services.TryAddSingleton(
            serviceType,
            _ => Activator.CreateInstance(implementationType, dict, fallback) ??
                   throw new InvalidOperationException());

        return this;
    }
}