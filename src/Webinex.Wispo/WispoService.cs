using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Coded;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Ports;
using Webinex.Wispo.Services;

namespace Webinex.Wispo;

internal class WispoService<TData> : IWispo<TData>
{
    private readonly IQueryService<TData> _queryService;
    private readonly IReadService _readService;
    private readonly IWispoDbContext<TData> _dbContext;
    private readonly IWispoFeedbackPort<TData> _feedback;

    public WispoService(
        IQueryService<TData> queryService,
        IReadService readService,
        IWispoDbContext<TData> dbContext,
        IWispoFeedbackPort<TData> feedback)
    {
        _queryService = queryService;
        _readService = readService;
        _dbContext = dbContext;
        _feedback = feedback;
    }

    public async Task<Notification<TData>[]> AddRangeAsync(AddArgs<TData>[] args)
    {
        args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));

        var rows = args
            .Select(x => NotificationRow<TData>.New(x.Type, x.RecipientId, x.Data, DateTimeOffset.UtcNow)).ToArray();
        await _dbContext.Notifications.AddRangeAsync(rows);
        var notifications = rows.Select(x => x.ToNotification()).ToArray();
        await _feedback.SendNewAsync(notifications);
        return notifications;
    }

    public async Task<IDictionary<Query, QueryResult<TData>>> QueryAsync(Query[] queries)
    {
        queries = queries ?? throw new ArgumentNullException(nameof(queries));

        var results = new List<KeyValuePair<Query, QueryResult<TData>>>();

        // TODO: improve to avoid cycle
        foreach (var query in queries)
            results.Add(KeyValuePair.Create(query, await _queryService.QueryAsync(query)));

        return new Dictionary<Query, QueryResult<TData>>(results);
    }

    public async Task<Notification<TData>> GetAsync(Guid id, Account? onBehalfOf = null)
    {
        var row = await _dbContext.Notifications.ById(id);
        if (onBehalfOf != null && !onBehalfOf.All.Contains(row.RecipientId, StringComparer.InvariantCultureIgnoreCase))
            throw CodedException.Unauthorized(id);
        return row.ToNotification();
    }

    public async Task ReadAsync(IEnumerable<ReadArgs> args)
    {
        args = args ?? throw new ArgumentNullException(nameof(args));
        await _readService.ReadAsync(args.ToArray());
    }

    public async Task ReadAllAsync(IEnumerable<Account> onBehalfOf)
    {
        await _readService.ReadAllAsync(onBehalfOf.ToArray());
    }
}