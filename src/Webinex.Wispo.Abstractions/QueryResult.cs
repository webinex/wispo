namespace Webinex.Wispo;

/// <summary>
///     Get notification query result
/// </summary>
public class QueryResult<TData>
{
    public QueryResult(
        Notification<TData>[]? items,
        int? totalUnreadCount,
        int? totalCount,
        int? totalMatchCount)
    {
        Items = items;
        TotalUnreadCount = totalUnreadCount;
        TotalCount = totalCount;
        TotalMatchCount = totalMatchCount;
    }

    /// <summary>
    ///     Notification items which matches filter criteria.
    ///     When <see cref="Query.Props"/> hasn't flag <see cref="QueryProp.Items"/>, value would be null.
    /// </summary>
    public Notification<TData>[]? Items { get; }

    /// <summary>
    ///     Unread notification count for specified user.
    ///     When <see cref="Query.Props"/> hasn't flag <see cref="QueryProp.TotalUnreadCount"/>, value would be null.
    /// </summary>
    public int? TotalUnreadCount { get; }

    /// <summary>
    ///     Total notifications count for specified user.
    ///     When <see cref="QueryProp.TotalCount"/> hasn't flag <see cref="QueryProp.TotalCount"/>, value would be null.
    /// </summary>
    public int? TotalCount { get; }

    /// <summary>
    ///     Total notifications count matching filter criteria for specified user.
    ///     When <see cref="QueryProp.TotalMatchCount"/> hasn't flag <see cref="QueryProp.TotalMatchCount"/>, value would be null.
    /// </summary>
    public int? TotalMatchCount { get; }
}