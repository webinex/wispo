using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webinex.Wispo;

/// <summary>
///     Wispo interactions facade
/// </summary>
public interface IWispo<TData>
{
    /// <summary>
    ///     Sends notifications
    /// </summary>
    /// <param name="args">Notification data payload</param>
    /// <returns><see cref="Task"/></returns>
    Task<Notification<TData>[]> AddRangeAsync(AddArgs<TData>[] args);

    /// <summary>
    ///     Gets notifications in accordance with <paramref name="queries"/> filter criteria.
    /// </summary>
    /// <param name="queries">Get arguments</param>
    /// <returns>Results by Recipient</returns>
    Task<IDictionary<Query, QueryResult<TData>>> QueryAsync(Query[] queries);

    /// <summary>
    ///     Gets one notification by identifier.
    ///     Throws exception if Recipient doesn't match.
    /// </summary>
    /// <param name="id">Notification identifier.</param>
    /// <param name="onBehalfOf">When specified, system would check user access</param>
    /// <returns>Found notification</returns>
    Task<Notification<TData>> GetAsync(Guid id, Account? onBehalfOf = null);

    /// <summary>
    ///     Marks notifications as read in accordance with <paramref name="args"/>.
    ///     When any of notification and recipient identifier doesn't match - exception thrown.
    /// </summary>
    /// <param name="args">Mark read arguments</param>
    /// <returns><see cref="Task"/></returns>
    Task ReadAsync(IEnumerable<ReadArgs> args);

    /// <summary>
    ///     Marks all notifications as read for specified recipients
    /// </summary>
    /// <param name="onBehalfOf">Recipients</param>
    /// <returns><see cref="Task"/></returns>
    Task ReadAllAsync(IEnumerable<Account> onBehalfOf);
}

public static class WispoExtensions
{
    public static async Task AddAsync<TData>(this IWispo<TData> wispo, AddArgs<TData> args)
    {
        wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
        args = args ?? throw new ArgumentNullException(nameof(args));
        await wispo.AddRangeAsync(new[] { args });
    }
    
    public static async Task ReadAllAsync<TData>(this IWispo<TData> wispo, Account onBehalfOf)
    {
        wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
        onBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));

        await wispo.ReadAllAsync(new[] { onBehalfOf });
    }

    public static async Task ReadAsync<TData>(
        this IWispo<TData> wispo,
        Account onBehalfOf,
        IEnumerable<Guid> ids)
    {
        wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
        onBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));
        ids = ids ?? throw new ArgumentNullException(nameof(ids));
        await wispo.ReadAsync(new[] { new ReadArgs(ids, onBehalfOf) });
    }

    public static async Task<QueryResult<TData>> QueryAsync<TData>(this IWispo<TData> wispo, Query query)
    {
        wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
        query = query ?? throw new ArgumentNullException(nameof(query));
        var results = await wispo.QueryAsync(new[] { query });
        return results.Values.Single();
    }

    public static async Task<int> TotalUnreadCountAsync<TData>(this IWispo<TData> wispo, Account onBehalfOf)
    {
        wispo = wispo ?? throw new ArgumentNullException(nameof(wispo));
        onBehalfOf = onBehalfOf ?? throw new ArgumentNullException(nameof(onBehalfOf));

        var result = await wispo.QueryAsync(new Query(QueryProp.TotalUnreadCount, onBehalfOf: onBehalfOf));
        return result.TotalUnreadCount!.Value;
    }
}