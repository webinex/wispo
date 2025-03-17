using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Wispo.Fcm.Devices;

public interface IWispoFcmDevicesService
{
    Task<WispoFcmDevice[]> GetAsync(IEnumerable<Guid> ids);

    Task<ILookup<string, WispoFcmDevice>> GetMapByRecipientIdAsync(IEnumerable<string> recipientIds,
        bool notStale = true);

    Task<WispoFcmDevice> AddOrUpdateAsync(string token, string recipientId, string? meta);
    Task<WispoFcmDevice[]> RemoveStaleAsync();
}

internal class WispoFcmDevicesService : IWispoFcmDevicesService
{
    private readonly IWispoFcmDevicesDbContext _dbContext;
    private readonly WispoFcmDevicesOptions _options;

    private DbSet<WispoFcmDevice> DbSet => _dbContext.WispoFcmDevices;

    private Expression<Func<WispoFcmDevice, bool>> GetIsStaleAtPredicate(DateTimeOffset at) =>
        e => e.UpdatedAt < at.Add(-_options.ConsiderStaleAfter);

    private Expression<Func<WispoFcmDevice, bool>> GetIsNotStaleAtPredicate(DateTimeOffset at) =>
        e => e.UpdatedAt >= at.Add(-_options.ConsiderStaleAfter);

    public WispoFcmDevicesService(IWispoFcmDevicesDbContext dbContext, WispoFcmDevicesOptions options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<WispoFcmDevice[]> GetAsync(IEnumerable<Guid> ids)
    {
        ids = ids.Distinct().ToArray();
        return await DbSet.Where(e => ids.Contains(e.Id)).ToArrayAsync();
    }

    public async Task<ILookup<string, WispoFcmDevice>> GetMapByRecipientIdAsync(
        IEnumerable<string> recipientIds,
        bool notStale = true)
    {
        recipientIds = recipientIds.Distinct().ToArray();

        var queryable = DbSet.Where(e => recipientIds.Contains(e.RecipientId));

        if (notStale)
            queryable = queryable.Where(GetIsNotStaleAtPredicate(DateTimeOffset.UtcNow));

        var tokens = await queryable.ToArrayAsync();
        return tokens.ToLookup(e => e.RecipientId);
    }

    public async Task<WispoFcmDevice> AddOrUpdateAsync(string token, string recipientId, string? meta)
    {
        var device = await DbSet.FirstOrDefaultAsync(e => e.Token == token);

        if (device != null)
        {
            device.Update(recipientId, meta);
        }
        else
        {
            device = new WispoFcmDevice(
                id: Guid.NewGuid(),
                token,
                recipientId,
                meta,
                updatedAt: DateTimeOffset.UtcNow,
                createdAt: DateTimeOffset.UtcNow);

            await DbSet.AddAsync(device);
        }

        return device;
    }

    public async Task<WispoFcmDevice[]> RemoveStaleAsync()
    {
        var stale = await DbSet.Where(GetIsStaleAtPredicate(DateTimeOffset.UtcNow)).ToArrayAsync();
        DbSet.RemoveRange(stale);

        return stale;
    }
}