using System;
using System.Diagnostics.CodeAnalysis;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.Ports
{
    public class NewNotificationFeedbackArgs
    {
        public NewNotificationFeedbackArgs([NotNull] NotificationRow notification, [NotNull] string[] accessibleBy)
        {
            Notification = notification ?? throw new ArgumentNullException(nameof(notification));
            AccessibleBy = accessibleBy ?? throw new ArgumentNullException(nameof(accessibleBy));
        }

        [NotNull]
        public NotificationRow Notification { get; }
        
        [NotNull]
        public string[] AccessibleBy { get; }
    }
}