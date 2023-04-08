using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo
{
    /// <summary>
    ///     Wispo group.
    ///     Allows to override values for group of recipients.
    /// 
    ///     Recipient values has top priority.
    ///     Then group values.
    ///     Then wispo values.
    /// </summary>
    public class WispoGroup
    {
        /// <summary>
        ///     Creates new instance of <see cref="WispoGroup"/>
        /// </summary>
        /// <param name="id">Group identifier</param>
        /// <param name="values">
        ///     Group values. Allows to override values for group of recipients.
        ///
        ///     Recipient values has top priority.
        ///     Then group values.
        ///     Then wispo values.
        /// </param>
        public WispoGroup([NotNull] string id, [NotNull] object values)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <summary>
        ///     Group identifier
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        ///     Group values
        /// </summary>
        public object Values { get; }
    }
}