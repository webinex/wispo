using System;
using System.Linq.Expressions;
using Webinex.Asky;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo;

internal class NotificationRowAskyFieldMap<TData> : IAskyFieldMap<NotificationRow<TData>>
{
    private readonly IAskyFieldMap<TData> _dataFieldMap;

    public NotificationRowAskyFieldMap(IAskyFieldMap<TData> dataFieldMap)
    {
        _dataFieldMap = dataFieldMap;
    }

    public Expression<Func<NotificationRow<TData>, object>>? this[string fieldId]
    {
        get
        {
            if (fieldId.StartsWith("data."))
                return AskyFieldMap.Forward<NotificationRow<TData>, TData>(
                    x => x.Data,
                    _dataFieldMap,
                    fieldId.Substring("data.".Length))!;

            return fieldId switch
            {
                "id" => x => x.Id,
                "recipientId" => x => x.RecipientId,
                "isRead" => x => x.IsRead,
                "createdAt" => x => x.CreatedAt,
                "readById" => x => x.ReadById!,
                "readAt" => x => x.ReadAt!,
                _ => null,
            };
        }
    }
}