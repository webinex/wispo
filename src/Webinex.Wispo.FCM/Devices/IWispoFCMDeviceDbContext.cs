using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.FCM.Devices;

public interface IWispoFCMDeviceDbContext
{
    DbSet<WispoFCMDevice> Devices { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}