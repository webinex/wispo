using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Filters;
using Webinex.Wispo.Ports;
using Webinex.Wispo.Services;
using Webinex.Wispo.Services.Templates;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Wispo configuration
    /// </summary>
    public interface IWispoConfiguration
    {
        /// <summary>
        ///     Service collection. Can be used in child packages.
        /// </summary>
        [NotNull]
        IServiceCollection Services { get; }
        
        /// <summary>
        ///     Values. Can be used in child packages.
        /// </summary>
        [NotNull]
        IDictionary<string, object> Values { get; }

        /// <summary>
        ///     Adds notification DAO services based on <typeparamref name="TDbContext"/> ef core context.
        /// </summary>
        /// <typeparam name="TDbContext">Type of DbContext to use</typeparam>
        /// <returns><see cref="IWispoConfiguration"/></returns>
        IWispoConfiguration AddDbContext<TDbContext>()
            where TDbContext : class, IWispoDbContext;
    }
    
    internal class WispoConfiguration : IWispoConfiguration
    {
        public WispoConfiguration(IServiceCollection services)
        {
            Services = services;

            services.TryAddScoped<IWispo, WispoFacade>();
            services.TryAddScoped<IValuesService, ValuesService>();
            services.TryAddScoped<ISendService, SendService>();
            services.TryAddScoped<IGetService, GetService>();
            services.TryAddScoped<IMarkReadService, MarkReadService>();
            services.TryAddScoped<IValidationService, ValidationService>();
            services.TryAddScoped<IWispoMapper, DefaultWispoMapper>();
            services.TryAddScoped<ITemplateService, DefaultTemplateService>();
            services.TryAddScoped<IFilterFactory, FilterFactory>();
            services.TryAddScoped<IFieldMap, DefaultFieldMap>();
            services.TryAddScoped<ISortService, SortService>();
        }

        public IServiceCollection Services { get; }
        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public IWispoConfiguration AddDbContext<TDbContext>() where TDbContext : class, IWispoDbContext
        {
            Services.TryAddScoped<IWispoDbContext, TDbContext>();
            return this;
        }

        public void Complete()
        {
            Services.TryAddSingleton<IWispoAccountAccessPort, DefaultWispoAccountAccessAdapter>();
        }
    }
}