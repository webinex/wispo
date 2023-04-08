using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo.DataAccess
{
    /// <summary>
    ///     Wispo notification entity
    /// </summary>
    public class NotificationRow
    {
        /// <summary>
        ///     Notification identifier
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        ///     Notification recipient
        /// </summary>
        public string RecipientId { get; set; }
        
        /// <summary>
        ///     Notification subject
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        ///     Notification body
        /// </summary>
        public string Body { get; set; }
        
        /// <summary>
        ///     True when notification read
        /// </summary>
        public bool IsRead { get; set; }
        
        /// <summary>
        ///     Date and time in UTC when notification created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        ///     ID of user which marked notification as read.  
        ///     Useful for notifications to shared accounts.  
        /// </summary>
        public string ReadById { get; set; }
        
        /// <summary>
        ///     Date and time in UTC when notification marked as read.  
        /// </summary>
        public DateTime? ReadAt { get; set; }

        public void Read([NotNull] string readById)
        {
            readById = readById ?? throw new ArgumentNullException(nameof(readById));

            IsRead = true;
            ReadById = readById;
            ReadAt = DateTime.UtcNow;
        }
    }
}