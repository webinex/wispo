using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo
{
    public class Notification
    {
        public Notification(Guid id, [NotNull] string subject, [NotNull] string body, bool read, DateTime createdAt, DateTime? readAt, string readById)
        {
            Id = id;
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Read = read;
            CreatedAt = createdAt;
            ReadAt = readAt;
            ReadById = readById;
        }

        /// <summary>
        ///     Notification identifier
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        ///     Notification subject
        /// </summary>
        public string Subject { get; }
        
        /// <summary>
        ///     Notification body
        /// </summary>
        public string Body { get; }
        
        /// <summary>
        ///     True when notification read
        /// </summary>
        public bool Read { get; }
        
        /// <summary>
        ///     Date and time when notification created in UTC
        /// </summary>
        public DateTime CreatedAt { get; }
        
        /// <summary>
        ///     Date and time when notification marked as read
        /// </summary>
        public DateTime? ReadAt { get; }
        
        /// <summary>
        ///     ID of user which marked notification as read.
        ///     Useful for shared accounts.
        /// </summary>
        public string ReadById { get; }
    }
}