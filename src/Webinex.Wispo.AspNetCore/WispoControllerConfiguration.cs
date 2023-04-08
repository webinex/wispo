using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Wispo.AspNetCore
{
    /// <summary>
    ///     Wispo controller settings
    /// </summary>
    public interface IWispoControllerSettings
    {
        /// <summary>
        ///     Authorization policy to check.
        ///     When null - no authorization check.
        /// </summary>
        [MaybeNull]
        string Policy { get; }
        
        
        /// <summary>
        ///     Authentication schema to check.
        ///     When null - no authorization check.
        /// </summary>
        [MaybeNull]
        string Schema { get; }
    }
    
    public interface IWispoControllerConfiguration
    {
        /// <summary>
        ///     Can be used in child packages.
        /// </summary>
        IMvcBuilder MvcBuilder { get; }

        /// <summary>
        ///     Adds <see cref="IUserService"/> implementation to get current user recipient identifier.
        /// </summary>
        /// <typeparam name="T">Type of implementation</typeparam>
        /// <returns><see cref="IWispoControllerConfiguration"/></returns>
        IWispoControllerConfiguration AddUserService<T>()
            where T : class, IUserService;

        /// <summary>
        ///     Specifies policy to authorize requests.
        ///     When not called - no authorization.
        /// </summary>
        /// <param name="policyName">Authorization policy name</param>
        /// <param name="schemaName">Authentication policy</param>
        /// <returns><see cref="IWispoControllerConfiguration"/></returns>
        IWispoControllerConfiguration AddPolicy([NotNull] string policyName, string schemaName);
    }

    internal class WispoControllerConfiguration : IWispoControllerConfiguration, IWispoControllerSettings
    {
        public WispoControllerConfiguration(IMvcBuilder mvcBuilder)
        {
            MvcBuilder = mvcBuilder;

            MvcBuilder.AddController(typeof(WispoController));
            MvcBuilder.Services.TryAddSingleton<IWispoControllerSettings>(this);
        }

        public IMvcBuilder MvcBuilder { get; }

        public string Policy { get; private set; }
        public string Schema { get; private set; }

        public IWispoControllerConfiguration AddUserService<T>() where T : class, IUserService
        {
            MvcBuilder.Services.AddScoped<IUserService, T>();
            return this;
        }

        public IWispoControllerConfiguration AddPolicy(string policyName, string schemaName)
        {
            Policy = policyName ?? throw new ArgumentNullException(nameof(policyName));
            Schema = schemaName ?? throw new ArgumentNullException(nameof(schemaName));
            return this;
        }

        public void Complete()
        {
            MvcBuilder.Services.TryAddSingleton<IUserService, DefaultUserService>();
        }
    }
}