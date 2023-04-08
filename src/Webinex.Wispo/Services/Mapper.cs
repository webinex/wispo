using System.Linq;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.Services
{
    public interface IWispoMapper
    {
        Notification Map(NotificationRow row);
        Notification[] MapMany(NotificationRow[] rows);
    }

    internal class DefaultWispoMapper : IWispoMapper
    {
        public Notification Map(NotificationRow row)
        {
            return new Notification(row.Id, row.Subject, row.Body, row.IsRead, row.CreatedAt, row.ReadAt, row.ReadById);
        }

        public Notification[] MapMany(NotificationRow[] rows)
        {
            return rows.Select(Map).ToArray();
        }
    }
}