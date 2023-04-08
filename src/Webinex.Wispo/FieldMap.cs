using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.NotificationField;

namespace Webinex.Wispo
{
    internal interface IFieldMap
    {
        [NotNull] Expression<Func<NotificationRow, object>> this[[NotNull] string fieldId] { get; }
    }

    internal class DefaultFieldMap : IFieldMap
    {
        public Expression<Func<NotificationRow, object>> this[string fieldId]
        {
            get
            {
                fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
                
                return fieldId switch
                {
                    ID => x => x.Id,
                    RECIPIENT_ID => x => x.RecipientId,
                    SUBJECT => x => x.Subject,
                    BODY => x => x.Body,
                    IS_READ => x => x.IsRead,
                    CREATED_AT => x => x.CreatedAt,
                    READ_BY_ID => x => x.ReadById,
                    READ_AT => x => x.ReadAt,
                    _ => throw new ArgumentOutOfRangeException(nameof(fieldId)),
                };
            }
        }
    }
}