using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.AspNetCore;

/// <summary>
///     Wispo controller settings
/// </summary>
public interface IWispoAspNetCoreSettings
{
    /// <summary>
    ///     Authorization policy to check.
    ///     When null - no authorization check.
    /// </summary>
    string? Policy { get; }

    /// <summary>
    ///     Authentication schema to check.
    ///     When null - no authorization check.
    /// </summary>
    string? Schema { get; }
}

public interface IWispoAspNetCoreConfiguration
{
    /// <summary>
    ///     Can be used in child packages.
    /// </summary>
    IMvcBuilder MvcBuilder { get; }

    /// <summary>
    ///     Data type
    /// </summary>
    Type DataType { get; }

    /// <summary>
    ///     Adds <see cref="IWispoAspNetCoreContextService"/> implementation to get current user recipient identifier.
    /// </summary>
    /// <typeparam name="TService">Type of implementation</typeparam>
    /// <returns><see cref="IWispoAspNetCoreConfiguration"/></returns>
    IWispoAspNetCoreConfiguration AddContext<TService>()
        where TService : class, IWispoAspNetCoreContextService;

    /// <summary>
    ///     Specifies policy to authorize requests.
    ///     When not called - no authorization.
    /// </summary>
    /// <param name="policyName">Authorization policy name</param>
    /// <param name="schemaName">Authentication policy</param>
    /// <returns><see cref="IWispoAspNetCoreConfiguration"/></returns>
    IWispoAspNetCoreConfiguration UsePolicy(string policyName, string schemaName);
}

internal class WispoAspNetCoreConfiguration : IWispoAspNetCoreConfiguration, IWispoAspNetCoreSettings
{
    private WispoAspNetCoreConfiguration(IMvcBuilder mvcBuilder, Type dataType)
    {
        MvcBuilder = mvcBuilder;
        DataType = dataType;
        MvcBuilder.Services.TryAddSingleton<IWispoAspNetCoreSettings>(this);
        
        MvcBuilder.Services.TryAddTransient(
            typeof(IWispoAspNetCore),
            typeof(WispoAspNetCore<>).MakeGenericType(dataType));

        var featureProvider =
            new ControllerRegistrationFeatureProvider(typeof(WispoController<>).MakeGenericType(dataType));
        mvcBuilder.ConfigureApplicationPartManager(x => x.FeatureProviders.Add(featureProvider));
    }

    public IMvcBuilder MvcBuilder { get; }
    public Type DataType { get; }

    public string? Policy { get; private set; }
    public string? Schema { get; private set; }

    public IWispoAspNetCoreConfiguration AddContext<T>() where T : class, IWispoAspNetCoreContextService
    {
        MvcBuilder.Services.AddScoped<IWispoAspNetCoreContextService, T>();
        return this;
    }

    public IWispoAspNetCoreConfiguration UsePolicy(string policyName, string schemaName)
    {
        Policy = policyName ?? throw new ArgumentNullException(nameof(policyName));
        Schema = schemaName ?? throw new ArgumentNullException(nameof(schemaName));
        return this;
    }

    internal void Complete()
    {
        MvcBuilder.Services.TryAddSingleton<IWispoAspNetCoreContextService, DefaultWispoAspNetCoreContextService>();
        MvcBuilder.Services.TryAddSingleton(
            typeof(IWispoAspNetCoreMapper<>).MakeGenericType(DataType),
            typeof(DefaultWispoAspNetCoreMapper<>).MakeGenericType(DataType));
    }

    internal static WispoAspNetCoreConfiguration GetOrCreate(IMvcBuilder mvcBuilder, Type dataType)
    {
        var instance = (WispoAspNetCoreConfiguration?)mvcBuilder.Services.FirstOrDefault(x =>
                x.ImplementationInstance?.GetType() == typeof(WispoAspNetCoreConfiguration))
            ?.ImplementationInstance;

        if (instance != null)
            return instance;

        instance = new WispoAspNetCoreConfiguration(mvcBuilder, dataType);
        mvcBuilder.Services.AddSingleton(instance);
        return instance;
    }

    private class ControllerRegistrationFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type _controllerType;

        public ControllerRegistrationFeatureProvider(Type controllerType)
        {
            _controllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (feature.Controllers.Contains(_controllerType.GetTypeInfo()))
                return;

            feature.Controllers.Add(_controllerType.GetTypeInfo());
        }
    }
}