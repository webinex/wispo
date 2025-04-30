using System.Threading;
using System.Threading.Tasks;
using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM.AspNetCore;

public interface IWispoFCMDevicesApi
{
    Task RegisterDevice(WispoFCMRegisterDeviceDto dto);
}

internal class WispoFCMDevicesApi : IWispoFCMDevicesApi
{
    private readonly IWispoFCMDeviceService _devicesService;
    private readonly IWispoFCMDeviceDtoMapper _dtoMapper;
    private readonly IWispoFCMDeviceDbContext _dbContext;

    public WispoFCMDevicesApi(
        IWispoFCMDeviceService devicesService,
        IWispoFCMDeviceDtoMapper dtoMapper,
        IWispoFCMDeviceDbContext dbContext)
    {
        _devicesService = devicesService;
        _dtoMapper = dtoMapper;
        _dbContext = dbContext;
    }

    public async Task RegisterDevice(WispoFCMRegisterDeviceDto dto)
    {
        var args = await _dtoMapper.MapAsync(dto);
        await _devicesService.AddOrUpdateAsync(args);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
    }
}