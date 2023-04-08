using System.Linq;
using System.Threading.Tasks;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.Services
{
    internal interface IValidationService
    {
        Task ValidateRecipientAndThrow(NotificationRow[] rows, string userId);
        Task ValidateRecipientAndThrow(NotificationRow notification, string[] userIds);
        Task ValidateRecipientAndThrow(NotificationRow row, string userId);
    }
    
    internal class ValidationService : IValidationService
    {
        private readonly IWispoAccountAccessPort _accountAccessPort;

        public ValidationService(IWispoAccountAccessPort accountAccessPort)
        {
            _accountAccessPort = accountAccessPort;
        }

        public async Task ValidateRecipientAndThrow(NotificationRow[] rows, string userId)
        {
            var accessibleAccounts = await _accountAccessPort.GetAccountsAccountHasAccess(userId);
            
            if (rows.Any(n => !accessibleAccounts.Contains(n.RecipientId)))
                throw CodedExceptions.AnotherRecipient();
        }

        public Task ValidateRecipientAndThrow(NotificationRow notification, string[] userIds)
        {
            if (!userIds.Contains(notification.RecipientId))
            {
                throw CodedExceptions.AnotherRecipient();
            }
            
            return Task.CompletedTask;
        }

        public Task ValidateRecipientAndThrow(NotificationRow row, string userId)
        {
            return ValidateRecipientAndThrow(new[] { row }, userId);
        }
    }
}