using System;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Wispo.AspNetCore;

public static class WispoMvcBuilderExtensions
{
    public static IMvcBuilder AddWispoAspNetCore<TData>(
        this IMvcBuilder mvcBuilder,
        Action<IWispoAspNetCoreConfiguration> configure)
    {
        var configuration = WispoAspNetCoreConfiguration.GetOrCreate(mvcBuilder, typeof(TData));
        configure(configuration);
        configuration.Complete();
        return mvcBuilder;
    }

    public static IMvcBuilder AddWispoAspNetCore<TData>(
        this IMvcBuilder mvcBuilder)
    {
        return AddWispoAspNetCore<TData>(mvcBuilder, _ => { });
    }
}