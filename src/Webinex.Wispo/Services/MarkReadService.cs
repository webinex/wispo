using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.Services
{
    internal interface IMarkReadService
    {
        Task MarkReadAsync(MarkReadArgs[] args);
        Task MarkAllReadAsync(string[] recipientIds);
    }

    internal class MarkReadService : IMarkReadService
    {
        private readonly IWispoDbContext _dbContext;
        private readonly IWispoFeedbackPort _feedbackPort;
        private readonly IValidationService _validationService;
        private readonly IWispoAccountAccessPort _accountAccessPort;

        public MarkReadService(
            IWispoDbContext dbContext,
            IWispoFeedbackPort feedbackPort,
            IValidationService validationService,
            IWispoAccountAccessPort accountAccessPort)
        {
            _dbContext = dbContext;
            _feedbackPort = feedbackPort;
            _validationService = validationService;
            _accountAccessPort = accountAccessPort;
        }

        public async Task MarkReadAsync(MarkReadArgs[] args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            if (!args.Any())
                return;

            var notifications = await MarkReadInternalAsync(args);
            await _dbContext.SaveChangesAsync();
            await SendReadFeedbackAsync(notifications);
        }

        private async Task<NotificationRow[]> MarkReadInternalAsync(MarkReadArgs[] args)
        {
            var recipientIds = args.Select(x => x.RecipientId).Distinct().ToArray();
            var accessibleAccountsByRecipientId = await _accountAccessPort.GetAccountsAccountHasAccess(recipientIds);
            var notificationIds = args.SelectMany(x => x.NotificationIds).ToArray();
            var notifications = await _dbContext.Notifications.ByIds(notificationIds).Unread().ToArrayAsync();

            foreach (var notification in notifications)
            {
                var recipientId = args.First(x => x.NotificationIds.Contains(notification.Id)).RecipientId;
                var accessibleAccounts = accessibleAccountsByRecipientId[recipientId];
                await _validationService.ValidateRecipientAndThrow(notification, accessibleAccounts);
                notification.Read(recipientId);
            }

            return notifications;
        }

        public async Task MarkAllReadAsync(string[] recipientIds)
        {
            recipientIds = recipientIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(recipientIds));

            if (!recipientIds.Any())
                return;

            var accessibleAccountsByAccountId = await _accountAccessPort.GetAccountsAccountHasAccess(recipientIds);
            var accountIds = accessibleAccountsByAccountId.SelectMany(x => x.Value);
            var notifications = await _dbContext.Notifications.ByUserIds(accountIds).Unread().ToArrayAsync();

            foreach (var notification in notifications)
            {
                var recipient = recipientIds.Contains(notification.RecipientId)
                    ? notification.RecipientId
                    : accessibleAccountsByAccountId.First(x => x.Value.Contains(notification.RecipientId)).Key;

                notification.Read(recipient);
            }

            await _dbContext.SaveChangesAsync();
            await SendReadFeedbackAsync(notifications);
        }

        private async Task SendReadFeedbackAsync(NotificationRow[] notifications)
        {
            var recipientIds = notifications.Select(x => x.RecipientId).Distinct().ToArray();
            var accountsHasAccessToByRecipientId = await _accountAccessPort.GetAccountsWhichHasAccessToAccount(recipientIds);
            var readFeedbackArgs = notifications.Select(x =>
                new ReadNotificationFeedbackArgs(x, accountsHasAccessToByRecipientId[x.RecipientId]));
            await _feedbackPort.SendReadAsync(readFeedbackArgs);
        }
    }
}