using System;
using Microsoft.AspNetCore.SignalR;

namespace Webinex.Wispo.SignalR;

public static class WispoConfigurationExtensions
{
    /// <summary>
    ///     Adds SignalR feedback service to Wispo
    /// </summary>
    /// <param name="configuration"><see cref="IWispoConfiguration"/></param>
    /// <typeparam name="THub">Type of Hub to send feedback</typeparam>
    /// <returns><see cref="IWispoConfiguration"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IWispoConfiguration AddSignalRFeedback<THub>(
        this IWispoConfiguration configuration)
        where THub : Hub
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        configuration.AppendFeedbackAdapter(
            typeof(SignalRHubWispoFeedbackAdapter<,>).MakeGenericType(configuration.DataType, typeof(THub)));

        return configuration;
    }
}