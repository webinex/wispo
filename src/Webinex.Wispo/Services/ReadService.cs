using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Coded;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.Services;

internal interface IReadService
{
    Task ReadAsync(ReadArgs[] args);
    Task ReadAllAsync(Account[] onBehalfOf);
}

internal class ReadService<TData> : IReadService
{
    private readonly IWispoDbContext<TData> _dbContext;
    private readonly IWispoFeedbackPort<TData> _feedbackPort;

    public ReadService(
        IWispoDbContext<TData> dbContext,
        IWispoFeedbackPort<TData> feedbackPort)
    {
        _dbContext = dbContext;
        _feedbackPort = feedbackPort;
    }

    public async Task ReadAsync(ReadArgs[] args)
    {
        args = args ?? throw new ArgumentNullException(nameof(args));
        if (!args.Any())
            return;

        var notifications = await MarkReadInternalAsync(args);
        await _dbContext.SaveChangesAsync();
        await SendReadFeedbackAsync(notifications);
    }

    private async Task<NotificationRow<TData>[]> MarkReadInternalAsync(ReadArgs[] args)
    {
        var ids = args.SelectMany(x => x.Id).ToArray();
        var rows = await _dbContext.Notifications.ById(ids).IsRead(false).ToArrayAsync();

        foreach (var row in rows)
        {
            var arg = args.First(x => x.Id.Contains(row.Id));
            if (!arg.OnBehalfOf.All.Contains(row.RecipientId, StringComparer.InvariantCultureIgnoreCase))
                throw CodedException.Unauthorized();

            row.Read(arg.OnBehalfOf.Id);
        }

        return rows;
    }

    public async Task ReadAllAsync(Account[] onBehalfOf)
    {
        onBehalfOf = onBehalfOf?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(onBehalfOf));

        if (!onBehalfOf.Any())
            return;

        var rows = await _dbContext.Notifications
            .ByRecipientId(onBehalfOf.SelectMany(x => x.All))
            .IsRead(false)
            .ToArrayAsync();

        foreach (var row in rows)
        {
            var account = onBehalfOf.FirstOrDefault(x => x.Id == row.RecipientId)
                          ?? onBehalfOf.First(x => x.All.Contains(row.RecipientId));
            row.Read(account.Id);
        }

        await _dbContext.SaveChangesAsync();
        await SendReadFeedbackAsync(rows);
    }

    private async Task SendReadFeedbackAsync(NotificationRow<TData>[] rows)
    {
        var notifications = rows.Select(x => x.ToNotification());
        await _feedbackPort.SendReadAsync(notifications);
    }
}