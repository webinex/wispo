using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Asky;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Services;

namespace Webinex.Wispo;

/// <summary>
///     Wispo configuration
/// </summary>
public interface IWispoConfiguration
{
    /// <summary>
    ///     Service collection. Can be used in child packages.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    ///     Data type
    /// </summary>
    Type DataType { get; }

    /// <summary>
    ///     Values. Can be used in child packages.
    /// </summary>
    IDictionary<string, object> Values { get; }

    /// <summary>
    ///     Adds notification DAO services based on <typeparamref name="TDbContext"/> ef core context.
    /// </summary>
    /// <typeparam name="TDbContext">Type of DbContext to use</typeparam>
    /// <returns><see cref="IWispoConfiguration"/></returns>
    IWispoConfiguration AddDbContext<TDbContext>()
        where TDbContext : class;
}

internal class WispoConfiguration : IWispoConfiguration
{
    private WispoConfiguration(IServiceCollection services, Type dataType)
    {
        Services = services;
        DataType = dataType;

        services.TryAddScoped(
            typeof(IWispo<>).MakeGenericType(DataType),
            typeof(WispoService<>).MakeGenericType(DataType));
        services.TryAddScoped(
            typeof(IQueryService<>).MakeGenericType(DataType),
            typeof(QueryService<>).MakeGenericType(DataType));
        services.TryAddScoped(
            typeof(IReadService),
            typeof(ReadService<>).MakeGenericType(DataType));
        services.TryAddScoped(
            typeof(IAskyFieldMap<>).MakeGenericType(typeof(NotificationRow<>).MakeGenericType(DataType)),
            typeof(NotificationRowAskyFieldMap<>).MakeGenericType(DataType));
    }

    public IServiceCollection Services { get; }
    public Type DataType { get; }
    public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

    public IWispoConfiguration AddDbContext<TDbContext>() where TDbContext : class
    {
        var serviceType = typeof(IWispoDbContext<>).MakeGenericType(DataType);
        if (!serviceType.IsAssignableFrom(typeof(TDbContext)))
            throw new InvalidOperationException($"{nameof(TDbContext)} might be assignable to {serviceType.FullName}");

        Services.TryAddScoped(serviceType, typeof(TDbContext));
        return this;
    }

    public void Complete()
    {
        Services.TryAddSingleton(
            typeof(IAskyFieldMap<>).MakeGenericType(DataType),
            typeof(EmptyAskyFieldMap<>).MakeGenericType(DataType));
    }

    internal static WispoConfiguration GetOrCreate(IServiceCollection services, Type dataType)
    {
        var instance = (WispoConfiguration?)services.FirstOrDefault(x =>
                x.ImplementationInstance?.GetType() == typeof(WispoConfiguration))
            ?.ImplementationInstance;

        if (instance != null)
            return instance;

        instance = new WispoConfiguration(services, dataType);
        services.AddSingleton(instance);
        return instance;
    }
}