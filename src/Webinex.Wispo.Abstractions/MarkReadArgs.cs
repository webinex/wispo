using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.Wispo
{
    public class MarkReadArgs
    {
        public MarkReadArgs([NotNull] string recipientId, [NotNull] IEnumerable<Guid> notificationIds)
        {
            RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
            NotificationIds = notificationIds?.ToArray() ?? throw new ArgumentNullException(nameof(notificationIds));
        }

        /// <summary>
        ///     Recipient identifier
        /// </summary>
        [NotNull]
        public string RecipientId { get; }
        
        /// <summary>
        ///     Notification identifiers
        /// </summary>
        [NotNull]
        public Guid[] NotificationIds { get; }
    }
}