using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo;

public static class WispoConfigurationExtensions
{
    /// <summary>
    /// Adds or appends feedback adapter to the Wispo configuration.
    /// Every new feedback will be added via decoration of already existing last one.
    /// </summary>
    public static IWispoConfiguration AppendFeedbackAdapter(this IWispoConfiguration @this, Type adapterType)
    {
        var serviceType = typeof(IWispoFeedbackPort<>).MakeGenericType(@this.DataType);

        if (!adapterType.IsAssignableTo(serviceType))
            throw new ArgumentException(
                $"Feedback adapter type '{adapterType.Name}' must implement 'IWispoFeedbackPort<>' interface");

        if (@this.Services.All(x => x.ServiceType != serviceType))
        {
            var defaultImplementation = typeof(EmptyWispoFeedbackAdapter<>).MakeGenericType(@this.DataType);
            @this.Services.AddScoped(serviceType, defaultImplementation);
        }

        @this.Services.Decorate(serviceType, adapterType);

        return @this;
    }
}

internal class EmptyWispoFeedbackAdapter<TData> : IWispoFeedbackPort<TData>
{
    public Task SendNewAsync(IEnumerable<Notification<TData>> notifications)
    {
        return Task.CompletedTask;
    }

    public Task SendReadAsync(IEnumerable<Notification<TData>> notifications)
    {
        return Task.CompletedTask;
    }
}