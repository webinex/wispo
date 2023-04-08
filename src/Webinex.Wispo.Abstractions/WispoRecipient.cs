using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Wispo recipient
    /// </summary>
    public class WispoRecipient
    {
        /// <summary>
        ///     Creates new instance of <see cref="WispoRecipient"/>
        /// </summary>
        /// <param name="id">Recipient identifier.</param>
        /// <param name="groupId">
        ///     Recipient group.
        ///     Allows to override values for group of users.</param>
        /// 
        ///     Recipient values has top priority.
        ///     Then group values.
        ///     Then wispo values.
        /// <param name="values">
        ///     Recipient values.
        /// 
        ///     Recipient values has top priority.
        ///     Then group values.
        ///     Then wispo values.
        /// </param>
        public WispoRecipient([NotNull] string id, string groupId = null, object values = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            GroupId = groupId;
            Values = values;
        }

        /// <summary>
        ///     Recipient identifier
        /// </summary>
        [NotNull]
        public string Id { get; }
        
        /// <summary>
        ///     Recipient values.
        ///     
        ///     Recipient values has top priority.
        ///     Then group values.
        ///     Then wispo values.
        /// </summary>
        [MaybeNull]
        public object Values { get; }
        
        /// <summary>
        ///     Recipient group.
        ///     Allows to specify values for group of recipients.
        ///
        ///     Recipient values has top priority.
        ///     Then group values.
        ///     Then wispo values.
        /// </summary>
        [MaybeNull]
        public string GroupId { get; }
    }
}