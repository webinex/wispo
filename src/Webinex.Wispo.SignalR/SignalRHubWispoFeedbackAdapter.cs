using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Webinex.Wispo.Ports;
using Webinex.Wispo.Services;

namespace Webinex.Wispo.SignalR
{
    internal class SignalRHubWispoFeedbackAdapter<THub> : IWispoFeedbackPort
        where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;
        private readonly IWispoMapper _mapper;

        public SignalRHubWispoFeedbackAdapter(IHubContext<THub> hubContext, IWispoMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task SendNewAsync(IEnumerable<NewNotificationFeedbackArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));
            var recipients = args.SelectMany((arg) => arg.AccessibleBy).Distinct().ToArray();

            foreach (var recipient in recipients)
            {
                var feedbackArgsForRecipient = args.Where(x => x.AccessibleBy.Contains(recipient)).ToArray();
                var notificationIds = feedbackArgsForRecipient.Select(x => x.Notification.Id).ToArray();
                var mappedNotifications = _mapper.MapMany(feedbackArgsForRecipient.Select(x => x.Notification).ToArray());
                await _hubContext.Clients.User(recipient).SendAsync(WispoSignalRMessages.New, notificationIds.Length,
                    notificationIds, mappedNotifications);
            }
        }

        public async Task SendReadAsync(IEnumerable<ReadNotificationFeedbackArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));

            var recipients = args.SelectMany(x => x.AccessibleBy).Distinct().ToArray();
            foreach (var recipient in recipients)
            {
                var notifications = args.Where(x => x.AccessibleBy.Contains(recipient)).ToArray();
                var notificationIds = notifications.Select(x => x.Notification.Id).ToArray();

                await _hubContext.Clients.User(recipient)
                    .SendAsync(WispoSignalRMessages.Read, notificationIds.Length, notificationIds);
            }
        }
    }
}