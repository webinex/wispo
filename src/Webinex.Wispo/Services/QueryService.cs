using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Asky;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.Services;

internal interface IQueryService<TData>
{
    Task<QueryResult<TData>> QueryAsync(Query args);
}

internal class QueryService<TData> : IQueryService<TData>
{
    private readonly IWispoDbContext<TData> _dbContext;
    private readonly IAskyFieldMap<NotificationRow<TData>> _askyFieldMap;

    public QueryService(
        IWispoDbContext<TData> dbContext,
        IAskyFieldMap<NotificationRow<TData>> askyFieldMap)
    {
        _dbContext = dbContext;
        _askyFieldMap = askyFieldMap;
    }

    public async Task<QueryResult<TData>> QueryAsync(Query args)
    {
        args = args ?? throw new ArgumentNullException(nameof(args));

        var notifications = await GetItemsAsync(args);
        var totalUnread = await GetTotalUnreadAsync(args);
        var total = await GetTotalAsync(args);
        var totalMatch = await GetTotalMatchAsync(args, total);
        return new QueryResult<TData>(notifications, totalUnread, total, totalMatch);
    }

    private async Task<Notification<TData>[]?> GetItemsAsync(Query args)
    {
        if (!args.Props.HasFlag(QueryProp.Items))
            return null;

        var queryable = _dbContext.Notifications
            .AsQueryable();

        if (args.OnBehalfOf != null)
            queryable = queryable.ByRecipientId(args.OnBehalfOf.All);

        if (args.SortRule?.Any() == true)
        {
            queryable = queryable.SortBy(_askyFieldMap, args.SortRule);
        }

        if (args.FilterRule != null)
        {
            queryable = queryable.Where(_askyFieldMap, args.FilterRule);
        }

        if (args.PagingRule != null)
        {
            queryable = queryable.PageBy(args.PagingRule);
        }

        var rows = await queryable.ToArrayAsync();
        return rows.Select(x => x.ToNotification()).ToArray();
    }

    private async Task<int?> GetTotalAsync(Query args, bool force = false)
    {
        if (!args.Props.HasFlag(QueryProp.TotalCount) && !force)
            return default;

        var queryable = _dbContext.Notifications.AsQueryable();

        if (args.OnBehalfOf != null)
            queryable = queryable.ByRecipientId(args.OnBehalfOf.All);

        return await queryable.CountAsync();
    }

    private async Task<int?> GetTotalUnreadAsync(Query args)
    {
        if (!args.Props.HasFlag(QueryProp.TotalUnreadCount))
            return default;

        var queryable = _dbContext.Notifications.AsQueryable().IsRead(false);

        if (args.OnBehalfOf != null)
            queryable = queryable.ByRecipientId(args.OnBehalfOf.All);

        return await queryable.CountAsync();
    }

    private async Task<int?> GetTotalMatchAsync(Query args, int? total)
    {
        if (!args.Props.HasFlag(QueryProp.TotalMatchCount))
            return null;

        if (args.FilterRule == null && total.HasValue)
            return total;

        if (args.FilterRule == null)
            return await GetTotalAsync(args, force: true);

        var queryable = _dbContext.Notifications.AsQueryable()
            .Where(_askyFieldMap, args.FilterRule);

        if (args.OnBehalfOf != null)
            queryable = queryable.ByRecipientId(args.OnBehalfOf.All);

        return await queryable.CountAsync();
    }
}