using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.Fcm.Devices;

public interface IWispoFcmDevicesDbContext
{
    DbSet<WispoFcmDevice> WispoFcmDevices { get; }
    Task SaveChangesAsync();
}