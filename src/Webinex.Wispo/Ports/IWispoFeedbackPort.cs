using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webinex.Wispo.Ports;

/// <summary>
///     Wispo feedback port.
///     Allows to receive callbacks when notifications mutated.
/// </summary>
public interface IWispoFeedbackPort<TData>
{
    /// <summary>
    ///     Invoked when new notifications sent.
    /// </summary>
    /// <param name="notifications">Sent notifications info</param>
    /// <returns><see cref="Task"/></returns>
    Task SendNewAsync(IEnumerable<Notification<TData>> notifications);
        
    /// <summary>
    ///     Invoked when notifications read.
    ///     Will not be called when ReadAll called.
    /// </summary>
    /// <param name="notifications">Read notifications info</param>
    /// <returns><see cref="Task"/></returns>
    Task SendReadAsync(IEnumerable<Notification<TData>> notifications);
}