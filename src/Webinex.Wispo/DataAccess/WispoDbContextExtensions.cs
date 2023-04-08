using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Wispo.DataAccess
{
    internal static class WispoDbContextExtensions
    {
        public static IQueryable<NotificationRow> ByUserIds(
            this IQueryable<NotificationRow> queryable,
            IEnumerable<string> userIds)
        {
            return queryable.Where(x => userIds.Contains(x.RecipientId));
        }

        public static IQueryable<NotificationRow> Unread(
            this IQueryable<NotificationRow> queryable)
        {
            return queryable.Where(x => !x.IsRead);
        }

        public static IQueryable<NotificationRow> ByIds(
            this IQueryable<NotificationRow> queryable,
            Guid[] ids)
        {
            return queryable.Where(x => ids.Contains(x.Id));
        }
    }
}