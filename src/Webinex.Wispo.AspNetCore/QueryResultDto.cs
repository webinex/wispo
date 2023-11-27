using System.Collections.Generic;

namespace Webinex.Wispo.AspNetCore;

public class QueryResultDto
{
    public QueryResultDto(
        IReadOnlyCollection<INotificationBase>? items,
        int? totalUnreadCount,
        int? totalCount,
        int? totalMatchCount)
    {
        Items = items;
        TotalUnreadCount = totalUnreadCount;
        TotalCount = totalCount;
        TotalMatchCount = totalMatchCount;
    }

    public IReadOnlyCollection<object>? Items { get; }
    public int? TotalUnreadCount { get; }
    public int? TotalCount { get; }
    public int? TotalMatchCount { get; }
}