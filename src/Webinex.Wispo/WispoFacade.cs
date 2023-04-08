using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Wispo.Services;

namespace Webinex.Wispo
{
    internal class WispoFacade : IWispo
    {
        private readonly ISendService _sendService;
        private readonly IGetService _getService;
        private readonly IMarkReadService _markReadService;

        public WispoFacade(ISendService sendService, IGetService getService, IMarkReadService markReadService)
        {
            _sendService = sendService;
            _getService = getService;
            _markReadService = markReadService;
        }

        public async Task<Notification[]> SendAsync(WispoArgs[] args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return await _sendService.SendAsync(args);
        }

        public async Task<IDictionary<string, GetNotificationsResult>> GetAsync(GetNotificationArgs[] args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            var result = new Dictionary<string, GetNotificationsResult>();
            foreach (var arg in args)
            {
                var queryResult = await _getService.GetAsync(arg);
                result.Add(arg.RecipientId, queryResult);
            }

            return result;
        }

        public Task<Notification> GetAsync(string recipientId, Guid id)
        {
            recipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));

            return _getService.GetAsync(recipientId, id);
        }

        public async Task<IDictionary<string, Guid[]>> GetUnreadIdsAsync(IEnumerable<string> recipientIds)
        {
            recipientIds = recipientIds?.ToArray() ?? throw new ArgumentNullException(nameof(recipientIds));
            if (!recipientIds.Any()) return new Dictionary<string, Guid[]>();
            
            var result = new Dictionary<string, Guid[]>();
            foreach (var recipientId in recipientIds)
            {
                var ids = await _getService.GetUnreadIdsAsync(recipientId);
                result.Add(recipientId, ids);
            }

            return result;
        }

        public async Task MarkReadAsync(IEnumerable<MarkReadArgs> args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            await _markReadService.MarkReadAsync(args.ToArray());
        }

        public async Task MarkAllReadAsync(IEnumerable<string> recipientIds)
        {
            recipientIds = recipientIds ?? throw new ArgumentNullException(nameof(recipientIds));
            await _markReadService.MarkAllReadAsync(recipientIds.ToArray());
        }
    }
}