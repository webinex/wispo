using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Wispo.AspNetCore
{
    internal static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddController<T>(this IMvcBuilder mvcBuilder)
        {
            return AddController(mvcBuilder, typeof(T));
        }
        
        public static IMvcBuilder AddController(this IMvcBuilder mvcBuilder, Type controllerType)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            controllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
            
            var featureProvider = new ControllerRegistrationFeatureProvider(controllerType);
            mvcBuilder.ConfigureApplicationPartManager(x => x.FeatureProviders.Add(featureProvider));

            return mvcBuilder;
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
}