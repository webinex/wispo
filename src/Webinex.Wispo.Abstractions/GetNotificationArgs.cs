using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Get notifications query arguments
    /// </summary>
    public class GetNotificationArgs
    {
        public GetNotificationArgs([NotNull] string recipientId)
        {
            RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
        }

        /// <summary>
        ///     Recipient identifier.
        ///     Result would be filtered for specified recipient identifier.
        /// </summary>
        [NotNull]
        public string RecipientId { get; }
        
        /// <summary>
        ///     <see cref="GetNotificationsResult.Items"/> would skip first specified number of items.
        ///     When null - no items would be skipped.
        /// </summary>
        public int? Skip { get; set; } = null;
        
        /// <summary>
        ///     <see cref="GetNotificationsResult.Items"/> length would be limited to specified number of items.
        ///     When null - all items items that matches filter criteria returned.
        /// </summary>
        public int? Take { get; set; } = null;
        
        /// <summary>
        ///     Specifies what should be included in result
        /// </summary>
        public Include Include { get; set; }
        
        /// <summary>
        ///     Specifies filtering criteria
        /// </summary>
        [MaybeNull]
        public Filter Filter { get; set; }
        
        /// <summary>
        ///     Specifies sorting criteria
        /// </summary>
        [MaybeNull]
        public SortRule[] Sort { get; set; }
    }
}