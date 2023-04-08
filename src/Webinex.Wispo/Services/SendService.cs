using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Ports;
using Webinex.Wispo.Services.Templates;

namespace Webinex.Wispo.Services
{
    internal interface ISendService
    {
        Task<Notification[]> SendAsync(WispoArgs[] args);
    }

    internal class SendService : ISendService
    {
        private readonly IValuesService _valuesService;
        private readonly IWispoDbContext _dbContext;
        private readonly IWispoFeedbackPort _feedbackPort;
        private readonly ITemplateService _templateService;
        private readonly IWispoAccountAccessPort _accountAccessPort;
        private readonly IWispoMapper _mapper;

        public SendService(
            IValuesService valuesService,
            IWispoDbContext dbContext,
            IWispoFeedbackPort feedbackPort,
            ITemplateService templateService,
            IWispoAccountAccessPort accountAccessPort,
            IWispoMapper mapper)
        {
            _valuesService = valuesService;
            _dbContext = dbContext;
            _feedbackPort = feedbackPort;
            _templateService = templateService;
            _accountAccessPort = accountAccessPort;
            _mapper = mapper;
        }

        public async Task<Notification[]> SendAsync(WispoArgs[] args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            if (!args.Any())
                return Array.Empty<Notification>();

            return await SendInternalAsync(args);
        }

        private async Task<Notification[]> SendInternalAsync(WispoArgs[] args)
        {
            var notifications = await CreateAsync(args);
            await _dbContext.SaveChangesAsync();
            await SendFeedbackAsync(notifications);
            return _mapper.MapMany(notifications);
        }

        private async Task SendFeedbackAsync(NotificationRow[] notifications)
        {
            var recipients = notifications.Select(x => x.RecipientId).Distinct().ToArray();
            var accountsHaveAccessByRecipientId = await _accountAccessPort.GetAccountsWhichHasAccessToAccount(recipients);
            var args = notifications.Select(x =>
                new NewNotificationFeedbackArgs(x, accountsHaveAccessByRecipientId[x.RecipientId]));
            await _feedbackPort.SendNewAsync(args);
        }

        private async Task<NotificationRow[]> CreateAsync(WispoArgs[] args)
        {
            var notifications = new LinkedList<NotificationRow>();
            foreach (var arg in args)
            {
                var created = await CreateAsync(arg);
                foreach (var row in created)
                    notifications.AddLast(row);
            }

            return notifications.ToArray();
        }

        private async Task<NotificationRow[]> CreateAsync(WispoArgs args)
        {
            var notifications = await NewManyAsync(args);
            await _dbContext.Notifications.AddRangeAsync(notifications);
            return notifications;
        }

        private async Task<NotificationRow[]> NewManyAsync(WispoArgs args)
        {
            var valuesByRecipientId = _valuesService.Merge(args);
            var results = await _templateService.RenderAsync(args, valuesByRecipientId);
            return results.Values.ToArray();
        }
    }
}