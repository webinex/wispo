using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Wispo.Fcm.Devices;

namespace Webinex.Wispo.Fcm;

public static class WispoConfigurationExtensions
{
    /// <summary>
    ///     Adds Firebase Cloud Messaging (FCM) feedback to Wispo
    /// </summary>
    /// <param name="configuration"><see cref="IWispoConfiguration"/></param>
    /// <param name="configure"><see cref="Action{IWispoFcmConfiguration}"/></param>
    /// <returns><see cref="IWispoConfiguration"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IWispoConfiguration AddFcmFeedback(
        this IWispoConfiguration configuration,
        Action<IWispoFcmConfiguration> configure)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        configure = configure ?? throw new ArgumentNullException(nameof(configure));

        var cfg = new WispoFcmConfiguration();
        configure(cfg);
        cfg.Validate();

        var services = configuration.Services;

        configuration
            .AddFeedbackAdapter(typeof(WispoFcmFeedbackAdapter<>).MakeGenericType(configuration.DataType));

        services.TryAddScoped(
            typeof(IWispoFcmMessageMapperFactory<>).MakeGenericType(configuration.DataType),
            typeof(DefaultWispoFcmMessageMapperFactory<>).MakeGenericType(configuration.DataType));
        services.TryAddScoped<IWispoFcmDevicesService, WispoFcmDevicesService>();
        services.TryAddScoped(typeof(IWispoFcmDevicesDbContext), cfg.DbContextType);

        services.AddScoped<IWispoFcmSender, WispoFcmSender>();
        services.AddSingleton(new WispoFcmOptions { FcmJsonCredentialData = cfg.FcmJsonCredentialData, });
        services.AddSingleton(new WispoFcmDevicesOptions { ConsiderStaleAfter = cfg.ConsiderStaleAfter, });

        return configuration;
    }
}

internal class WispoFcmOptions
{
    public string FcmJsonCredentialData { get; init; } = null!;
}

internal class WispoFcmDevicesOptions
{
    public TimeSpan ConsiderStaleAfter { get; init; }
}

public interface IWispoFcmConfiguration
{
    /// <summary>
    /// Sets FCM JSON credential data. This data will be used to communicate with FCM
    /// </summary>
    IWispoFcmConfiguration UseFcmJsonCredentialData(string data);

    /// <summary>
    /// Sets the period of inactivity after which devices will be considered stale. Must be positive and greater than 0.
    /// </summary>
    IWispoFcmConfiguration UseDevicesStalePeriod(TimeSpan period);

    /// <summary>
    /// Sets DbContext type to be used for storing FCM device tokens.
    /// </summary>
    IWispoFcmConfiguration AddDevicesDbContext(Type dbContext);

    // /// <summary>
    // /// Adds a job that will clean up stale devices from the database.
    // /// </summary>
    // IWispoFcmConfiguration AddStaleDevicesCleaningJob(); // TODO: add periodic
}

internal class WispoFcmConfiguration : IWispoFcmConfiguration
{
    public Type? DbContextType { get; set; }
    public string? FcmJsonCredentialData { get; set; }
    public TimeSpan ConsiderStaleAfter { get; set; } = TimeSpan.FromDays(30);

    [MemberNotNull(nameof(FcmJsonCredentialData), nameof(DbContextType))]
    public void Validate()
    {
        if (FcmJsonCredentialData == null)
            throw new InvalidOperationException("FcmJsonCredentialData must be provided");

        if (DbContextType == null)
            throw new InvalidOperationException("DbContextType must be provided");
    }

    public IWispoFcmConfiguration UseFcmJsonCredentialData(string data)
    {
        FcmJsonCredentialData = data ?? throw new ArgumentNullException(nameof(data));
        return this;
    }

    public IWispoFcmConfiguration UseDevicesStalePeriod(TimeSpan period)
    {
        if (period <= TimeSpan.Zero)
            throw new ArgumentException("Stale period must be positive and greater than 0");

        ConsiderStaleAfter = period;
        return this;
    }

    public IWispoFcmConfiguration AddDevicesDbContext(Type dbContext)
    {
        if (!dbContext.IsAssignableTo(typeof(IWispoFcmDevicesDbContext)))
            throw new ArgumentException($"dbContext must implements {nameof(IWispoFcmDevicesDbContext)}");

        DbContextType = dbContext;
        return this;
    }
}