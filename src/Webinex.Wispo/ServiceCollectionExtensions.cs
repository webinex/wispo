using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Wispo
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds Wispo services to <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="configure">Allows to configure Wispo</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddWispo(
            [NotNull] this IServiceCollection services,
            [NotNull] Action<IWispoConfiguration> configure)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new WispoConfiguration(services);
            configure(configuration);
            configuration.Complete();

            return services;
        }
    }
}