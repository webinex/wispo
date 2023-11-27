using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.DataAccess;

public interface IWispoDbContext<TData>
{
    DbSet<NotificationRow<TData>> Notifications { get; }
    Task SaveChangesAsync();
}