using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Coded;

namespace Webinex.Wispo.DataAccess;

internal static class DbContextExtensions
{
    public static IQueryable<NotificationRow<TData>> ByRecipientId<TData>(
        this IQueryable<NotificationRow<TData>> queryable,
        IEnumerable<string> recipientIds)
    {
        recipientIds = recipientIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(recipientIds));
        return queryable.Where(x => recipientIds.Contains(x.RecipientId));
    }

    public static IQueryable<NotificationRow<TData>> IsRead<TData>(
        this IQueryable<NotificationRow<TData>> queryable,
        bool value)
    {
        return queryable.Where(x => x.IsRead == value);
    }

    public static IQueryable<NotificationRow<TData>> ById<TData>(
        this IQueryable<NotificationRow<TData>> queryable,
        Guid[] ids)
    {
        return queryable.Where(x => ids.Contains(x.Id));
    }

    public static async Task<NotificationRow<TData>> ById<TData>(
        this DbSet<NotificationRow<TData>> dbSet,
        Guid id)
    {
        return await dbSet.FindAsync(id)
               ?? throw CodedException.NotFound(id);
    }
}