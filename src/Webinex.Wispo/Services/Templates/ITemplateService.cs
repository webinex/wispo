using System.Collections.Generic;
using System.Threading.Tasks;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.Services.Templates
{
    /// <summary>
    ///     Template service
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        ///     Creates new instances of <see cref="NotificationRow"/> based on
        ///     input <paramref name="args"/> and <paramref name="valuesByRecipientId"/>
        /// </summary>
        /// <param name="args">Wispo notification args</param>
        /// <param name="valuesByRecipientId">Merged values by recipient identifier</param>
        /// <returns>Notifications by recipient identifier</returns>
        Task<IDictionary<string, NotificationRow>> RenderAsync(WispoArgs args, IDictionary<string, object> valuesByRecipientId);
    }
}