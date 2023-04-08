using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Get notification query result
    /// </summary>
    public class GetNotificationsResult
    {
        public GetNotificationsResult(
            [MaybeNull] Notification[] items,
            [MaybeNull] int? totalUnread,
            [MaybeNull] int? total,
            [MaybeNull] int? totalMatch)
        {
            Items = items;
            TotalUnread = totalUnread;
            Total = total;
            TotalMatch = totalMatch;
        }

        /// <summary>
        ///     Notification items which matches filter criteria.
        ///     When <see cref="GetNotificationArgs.Include"/> hasn't flag <see cref="Include.Items"/>, value would be null.
        /// </summary>
        [MaybeNull] public Notification[] Items { get; }

        /// <summary>
        ///     Unread notification count for specified user.
        ///     When <see cref="GetNotificationArgs.Include"/> hasn't flag <see cref="Include.TotalUnread"/>, value would be null.
        /// </summary>
        [MaybeNull] public int? TotalUnread { get; }

        /// <summary>
        ///     Total notifications count for specified user.
        ///     When <see cref="GetNotificationArgs.Include"/> hasn't flag <see cref="Include.Total"/>, value would be null.
        /// </summary>
        [MaybeNull] public int? Total { get; }

        /// <summary>
        ///     Total notifications count matching filter criteria for specified user.
        ///     When <see cref="GetNotificationArgs.Include"/> hasn't flag <see cref="Include.TotalMatch"/>, value would be null.
        /// </summary>
        [MaybeNull] public int? TotalMatch { get; }
    }
}