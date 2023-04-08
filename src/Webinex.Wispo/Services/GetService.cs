using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Filters;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.Services
{
    internal interface IGetService
    {
        Task<GetNotificationsResult> GetAsync(GetNotificationArgs args);

        Task<Notification> GetAsync(string userId, Guid notificationId);

        Task<Guid[]> GetUnreadIdsAsync(string userId);
    }

    internal class GetService : IGetService
    {
        private readonly IWispoDbContext _dbContext;
        private readonly IValidationService _validationService;
        private readonly IWispoMapper _mapper;
        private readonly IWispoAccountAccessPort _accountAccessPort;
        private readonly IFilterFactory _filterFactory;
        private readonly ISortService _sortService;

        public GetService(
            IWispoDbContext dbContext,
            IValidationService validationService,
            IWispoMapper mapper,
            IWispoAccountAccessPort accountAccessPort,
            IFilterFactory filterFactory,
            ISortService sortService)
        {
            _dbContext = dbContext;
            _validationService = validationService;
            _mapper = mapper;
            _accountAccessPort = accountAccessPort;
            _filterFactory = filterFactory;
            _sortService = sortService;
        }

        public async Task<Notification> GetAsync(string userId, Guid notificationId)
        {
            userId = userId ?? throw new ArgumentNullException(nameof(userId));

            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            await _validationService.ValidateRecipientAndThrow(notification, userId);
            return _mapper.Map(notification);
        }

        public async Task<Guid[]> GetUnreadIdsAsync(string userId)
        {
            userId = userId ?? throw new ArgumentNullException(nameof(userId));
            var accessibleAccountIds = await _accountAccessPort.GetAccountsAccountHasAccess(userId);

            return await _dbContext.Notifications
                .ByUserIds(accessibleAccountIds)
                .Unread()
                .Select(x => x.Id)
                .ToArrayAsync();
        }

        public async Task<GetNotificationsResult> GetAsync(GetNotificationArgs args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            var accessibleAccounts = await _accountAccessPort.GetAccountsAccountHasAccess(args.RecipientId);
            var notifications = await GetItemsAsync(args, accessibleAccounts);
            var totalUnread = await GetTotalUnreadAsync(args, accessibleAccounts);
            var total = await GetTotalAsync(args, accessibleAccounts);
            var totalMatch = await GetTotalMatchAsync(args, accessibleAccounts, total);
            return new GetNotificationsResult(notifications, totalUnread, total, totalMatch);
        }

        private async Task<Notification[]> GetItemsAsync(GetNotificationArgs args, IEnumerable<string> accounts)
        {
            if (!args.Include.HasFlag(Include.Items))
                return null;

            var queryable = _dbContext.Notifications
                .ByUserIds(accounts)
                .AsQueryable();

            if (args.Sort?.Any() == true)
            {
                queryable = _sortService.Apply(queryable, args.Sort);
            }

            if (args.Filter != null)
            {
                queryable = queryable.Where(_filterFactory.Create(args.Filter));
            }

            if (args.Skip.HasValue)
            {
                queryable = queryable.Skip(args.Skip.Value);
            }

            if (args.Take.HasValue)
            {
                queryable = queryable.Take(args.Take.Value);
            }

            var items = await queryable.ToArrayAsync();
            return _mapper.MapMany(items);
        }

        private async Task<int?> GetTotalAsync(GetNotificationArgs args, IEnumerable<string> accounts)
        {
            return args.Include.HasFlag(Include.Total)
                ? await GetTotalAsync(accounts)
                : default(int?);
        }

        private async Task<int?> GetTotalUnreadAsync(GetNotificationArgs args, IEnumerable<string> accounts)
        {
            return args.Include.HasFlag(Include.TotalUnread)
                ? await GetTotalUnreadAsync(accounts)
                : default(int?);
        }

        private async Task<int> GetTotalAsync(IEnumerable<string> accounts)
        {
            return await _dbContext.Notifications.ByUserIds(accounts).CountAsync();
        }

        private async Task<int?> GetTotalMatchAsync(GetNotificationArgs args, IEnumerable<string> accounts, int? total)
        {
            if (!args.Include.HasFlag(Include.TotalMatch))
                return null;

            if (args.Filter == null && total.HasValue)
                return total;

            if (args.Filter == null)
                return await GetTotalAsync(accounts);

            return await _dbContext.Notifications.ByUserIds(accounts).Where(_filterFactory.Create(args.Filter))
                .CountAsync();
        }

        private async Task<int?> GetTotalUnreadAsync(IEnumerable<string> accounts)
        {
            return await _dbContext.Notifications.ByUserIds(accounts).Unread().CountAsync();
        }
    }
}