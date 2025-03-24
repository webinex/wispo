using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.FCM.Devices;

public interface IWispoFCMDevicesService
{
    Task<WispoFCMDevice[]> GetAllAsync(WispoFCMDevicesFilters filters);
    Task<WispoFCMDevice> AddOrUpdateAsync(WispoAddOrUpdateFCMDeviceArgs args);
    Task<WispoFCMDevice[]> RemoveStaleAsync();
    Task RemoveRangeAsync(IEnumerable<WispoFCMDevice> devices);
}

public class WispoFCMDevicesFilters
{
    public Guid[]? Ids { get; init; }
    public string[]? RecipientIds { get; init; }
    public bool? Stale { get; init; }

    public WispoFCMDevicesFilters(
        IEnumerable<Guid>? ids = null,
        IEnumerable<string>? recipientIds = null,
        bool? stale = null)
    {
        Ids = ids?.Distinct().ToArray();
        RecipientIds = recipientIds?.Distinct().ToArray();
        Stale = stale;
    }
}

public class WispoAddOrUpdateFCMDeviceArgs
{
    public string Token { get; init; }
    public string RecipientId { get; init; }
    public string? Meta { get; init; }

    public WispoAddOrUpdateFCMDeviceArgs(string token, string recipientId, string? meta)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
        Meta = meta;
    }
}

internal class WispoFCMDevicesService : IWispoFCMDevicesService
{
    private readonly IWispoFCMDevicesDbContext _dbContext;
    private readonly WispoFCMDevicesOptions _options;

    private DbSet<WispoFCMDevice> DbSet => _dbContext.WispoFCMDevices;

    private Expression<Func<WispoFCMDevice, bool>> GetIsStaleAtPredicate(DateTimeOffset at) =>
        e => e.UpdatedAt < at.Add(-_options.ConsiderStaleAfter);

    private Expression<Func<WispoFCMDevice, bool>> GetIsNotStaleAtPredicate(DateTimeOffset at) =>
        e => e.UpdatedAt >= at.Add(-_options.ConsiderStaleAfter);

    public WispoFCMDevicesService(IWispoFCMDevicesDbContext dbContext, WispoFCMDevicesOptions options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<WispoFCMDevice[]> GetAllAsync(WispoFCMDevicesFilters filters)
    {
        var queryable = DbSet.AsQueryable();
        queryable = Filter(queryable, filters);
        return await queryable.ToArrayAsync();
    }

    public async Task<WispoFCMDevice> AddOrUpdateAsync(WispoAddOrUpdateFCMDeviceArgs args)
    {
        var device = await DbSet.FirstOrDefaultAsync(e => e.Token == args.Token);

        if (device != null)
        {
            device.Update(args.RecipientId, args.Meta);
        }
        else
        {
            device = new WispoFCMDevice(
                id: Guid.NewGuid(),
                args.Token,
                args.RecipientId,
                args.Meta,
                updatedAt: DateTimeOffset.UtcNow,
                createdAt: DateTimeOffset.UtcNow);

            await DbSet.AddAsync(device);
        }

        return device;
    }

    public async Task<WispoFCMDevice[]> RemoveStaleAsync()
    {
        var stale = await GetAllAsync(new WispoFCMDevicesFilters(stale: true));
        await RemoveRangeAsync(stale);

        return stale;
    }

    public Task RemoveRangeAsync(IEnumerable<WispoFCMDevice> devices)
    {
        DbSet.RemoveRange(devices);
        return Task.CompletedTask;
    }

    private IQueryable<WispoFCMDevice> Filter(
        IQueryable<WispoFCMDevice> queryable,
        WispoFCMDevicesFilters filters)
    {
        if (filters.Ids?.Any() == true)
            queryable = queryable.Where(e => filters.Ids.Contains(e.Id));

        if (filters.RecipientIds?.Any() == true)
            queryable = queryable.Where(e => filters.RecipientIds.Contains(e.RecipientId));

        if (filters.Stale.HasValue)
            queryable = filters.Stale.Value
                ? queryable.Where(GetIsStaleAtPredicate(DateTimeOffset.UtcNow))
                : queryable.Where(GetIsNotStaleAtPredicate(DateTimeOffset.UtcNow));

        return queryable;
    }
}