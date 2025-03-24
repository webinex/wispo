using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webinex.Wispo.FCM.Devices;

public static class WispoFCMDevicesServiceExtensions
{
    public static async Task<ILookup<string, WispoFCMDevice>> GetMapByRecipientIdAsync(
        this IWispoFCMDevicesService @this,
        IEnumerable<string> recipientIds,
        bool stale = false)
    {
        var filters = new WispoFCMDevicesFilters(recipientIds: recipientIds, stale: stale);

        if (filters.RecipientIds == null || !filters.RecipientIds.Any())
            return Array.Empty<(string, WispoFCMDevice)>().ToLookup(e => e.Item1, e => e.Item2);

        var devices = await @this.GetAllAsync(filters);
        return devices.ToLookup(e => e.RecipientId);
    }
}