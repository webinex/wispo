using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.DataAccess
{
    public interface IWispoDbContext
    {
        DbSet<NotificationRow> Notifications { get; }

        Task SaveChangesAsync();
    }
}