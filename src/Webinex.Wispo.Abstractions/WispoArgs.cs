using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Notification args
    /// </summary>
    public class WispoArgs
    {
        /// <summary>
        ///     Creates new instance of <see cref="WispoArgs"/>
        /// </summary>
        /// <param name="subject">
        ///     Notification subject. Can be Scriban template.
        /// </param>
        /// <param name="body">
        ///     Notification body. Can be Scriban template.
        /// </param>
        /// <param name="recipients">
        ///     Notification recipients.
        /// </param>
        /// <param name="values">
        ///     Shared notification values. Has lowest priority.
        /// </param>
        /// <param name="groups">
        ///     Recipient groups. Allows to override values for group of users.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when recipients enumerable empty.
        /// </exception>
        public WispoArgs(
            [NotNull] string subject,
            [NotNull] string body,
            [NotNull] IEnumerable<WispoRecipient> recipients,
            object values = null,
            IEnumerable<WispoGroup> groups = null)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Recipients = recipients?.ToArray() ?? throw new ArgumentNullException(nameof(recipients));
            Values = values;
            Groups = groups?.ToArray() ?? Array.Empty<WispoGroup>();

            if (!Recipients.Any())
                throw new InvalidOperationException("Unable to send notification to 0 recipients");
        }

        /// <summary>
        ///     Notification subject.
        ///     Can be Scriban template.
        /// </summary>
        [NotNull]
        public string Subject { get; }
        
        /// <summary>
        ///     Notification body.
        ///     Can be Scriban template.
        /// </summary>
        [NotNull]
        public string Body { get; }
        
        /// <summary>
        ///     Notification values.
        ///     Values would be merged with group and recipient values.
        ///
        ///     Recipient values has top priority.
        ///     Then group values.
        ///     Then wispo values.
        /// </summary>
        [MaybeNull]
        public object Values { get; }
        
        /// <summary>
        ///     Recipient groups.
        ///     Allows to specify values for groups.
        /// </summary>
        [NotNull]
        public WispoGroup[] Groups { get; }
        
        /// <summary>
        ///     Notification recipients.
        /// </summary>
        [NotNull]
        public WispoRecipient[] Recipients { get; }
    }
}