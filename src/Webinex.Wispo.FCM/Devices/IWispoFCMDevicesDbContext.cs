using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.FCM.Devices;

public interface IWispoFCMDevicesDbContext
{
    DbSet<WispoFCMDevice> WispoFCMDevices { get; }
    Task SaveChangesAsync();
}