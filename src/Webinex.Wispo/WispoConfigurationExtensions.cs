using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo;

public static class WispoConfigurationExtensions
{
    public static IWispoConfiguration AddFeedbackAdapter(this IWispoConfiguration @this, Type adapterType)
    {
        var serviceType = typeof(IWispoFeedbackPort<>).MakeGenericType(@this.DataType);

        if (!adapterType.IsAssignableTo(serviceType))
            throw new ArgumentException(
                $"Feedback adapter type '{adapterType.Name}' must implement 'IWispoFeedbackPort<>' interface");

        var descriptor = @this.Services.SingleOrDefault(x => x.ServiceType == serviceType);

        if (descriptor == null)
        {
            @this.Services.AddScoped(serviceType, adapterType);
            return @this;
        }

        var originalFeedbackType = descriptor.ImplementationType ?? throw new ArgumentNullException();
        @this.Services.TryDecorate(originalFeedbackType, adapterType);

        return @this;
    }
}