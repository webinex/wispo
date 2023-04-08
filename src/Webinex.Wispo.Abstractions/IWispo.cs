using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Wispo interactions facade
    /// </summary>
    public interface IWispo
    {
        /// <summary>
        ///     Sends notifications
        /// </summary>
        /// <param name="args">Notification data payload</param>
        /// <returns><see cref="Task"/></returns>
        Task<Notification[]> SendAsync([NotNull] WispoArgs[] args);

        /// <summary>
        ///     Gets notifications in accordance with <paramref name="args"/> filter criteria.
        /// </summary>
        /// <param name="args">Get arguments</param>
        /// <returns>Results by Recipient identifiers</returns>
        Task<IDictionary<string, GetNotificationsResult>> GetAsync([NotNull] GetNotificationArgs[] args);

        /// <summary>
        ///     Gets one notification by identifier.
        ///     Throws exception if Recipient doesn't match.
        /// </summary>
        /// <param name="recipientId">Notification recipient identifier.</param>
        /// <param name="id">Notification identifier.</param>
        /// <returns>Found notification</returns>
        Task<Notification> GetAsync([NotNull] string recipientId, Guid id);

        /// <summary>
        ///     Gets unread notification identifiers for specified <paramref name="recipientIds"/>
        /// </summary>
        /// <param name="recipientIds">Recipient identifiers</param>
        /// <returns>Unread notification identifiers by recipient identifier</returns>
        Task<IDictionary<string, Guid[]>> GetUnreadIdsAsync([NotNull] IEnumerable<string> recipientIds);

        /// <summary>
        ///     Marks notifications as read in accordance with <paramref name="args"/>.
        ///     When any of notification and recipient identifier doesn't match - exception thrown.
        /// </summary>
        /// <param name="args">Mark read arguments</param>
        /// <returns><see cref="Task"/></returns>
        Task MarkReadAsync([NotNull] IEnumerable<MarkReadArgs> args);

        /// <summary>
        ///     Marks all notifications as read for specified recipients
        /// </summary>
        /// <param name="recipientIds">Recipient identifiers</param>
        /// <returns><see cref="Task"/></returns>
        Task MarkAllReadAsync([NotNull] IEnumerable<string> recipientIds);
    }

    public static class WispoExtensions
    {
        /// <summary>
        ///     Sends notifications
        /// </summary>
        /// <param name="wispo"><see cref="IWispo"/></param>
        /// <param name="args">Notification data payload</param>
        /// <returns><see cref="Task"/></returns>
        public static async Task<Notification> SendAsync([NotNull] this IWispo wispo, [NotNull] WispoArgs args)
        {
            wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
            args = args ?? throw new ArgumentNullException(nameof(args));

            var notifications = await wispo.SendAsync(new[] { args });
            return notifications.SingleOrDefault();
        }

        /// <summary>
        ///     Gets notifications in accordance with <paramref name="args"/> filter criteria.
        /// </summary>
        /// <param name="wispo"><see cref="IWispo"/></param>
        /// <param name="args">Get arguments</param>
        /// <returns>Results by Recipient identifiers</returns>
        public static async Task<GetNotificationsResult> GetAsync(
            [NotNull] this IWispo wispo,
            [NotNull] GetNotificationArgs args)
        {
            wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
            args = args ?? throw new ArgumentNullException(nameof(args));

            var result = await wispo.GetAsync(new[] { args });
            return result.Values.Single();
        }

        /// <summary>
        ///     Gets unread notification identifiers for specified <paramref name="recipientId"/>
        /// </summary>
        /// <param name="wispo"><see cref="IWispo"/></param>
        /// <param name="recipientId">Recipient identifier</param>
        /// <returns>Unread notification identifiers by recipient identifier</returns>
        public static async Task<Guid[]> GetUnreadIdsAsync(
            [NotNull] this IWispo wispo,
            [NotNull] string recipientId)
        {
            wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
            recipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));

            var result = await wispo.GetUnreadIdsAsync(new[] { recipientId });
            return result.Values.Single();
        }

        /// <summary>
        ///     Marks notifications <paramref name="notificationIds"/> as read
        ///     for <paramref name="recipientId"/>.
        ///     When any of notification and recipient identifier doesn't match - exception thrown.
        /// </summary>
        /// <param name="wispo"><see cref="IWispo"/></param>
        /// <param name="recipientId">Recipient identifier</param>
        /// <param name="notificationIds">Notification identifiers</param>
        /// <returns><see cref="Task"/></returns>
        public static async Task MarkReadAsync(
            [NotNull] this IWispo wispo,
            [NotNull] string recipientId,
            [NotNull] IEnumerable<Guid> notificationIds)
        {
            wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
            recipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
            notificationIds = notificationIds ?? throw new ArgumentNullException(nameof(notificationIds));

            await wispo.MarkReadAsync(new[] { new MarkReadArgs(recipientId, notificationIds) });
        }


        /// <summary>
        ///     Marks all notifications as read for specified recipient
        /// </summary>
        /// <param name="wispo"><see cref="IWispo"/></param>
        /// <param name="recipientId">Recipient identifier</param>
        /// <returns><see cref="Task"/></returns>
        public static async Task MarkAllReadAsync(
            [NotNull] this IWispo wispo,
            [NotNull] string recipientId)
        {
            wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
            recipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));

            await wispo.MarkAllReadAsync(new[] { recipientId });
        }
    }
}