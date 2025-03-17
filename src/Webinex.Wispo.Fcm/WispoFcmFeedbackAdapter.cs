using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Webinex.Wispo.Fcm.Devices;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.Fcm;

internal class WispoFcmFeedbackAdapter<TData> : IWispoFeedbackPort<TData>
{
    private readonly IWispoFeedbackPort<TData> _next;
    private readonly IWispoFcmDevicesService _devicesService;
    private readonly ILogger _logger;
    private readonly IWispoFcmMessageMapperFactory<TData> _messageMapperFactory;
    private readonly IWispoFcmSender _wispoFcmSender;

    public WispoFcmFeedbackAdapter(
        IWispoFeedbackPort<TData> next,
        IWispoFcmDevicesService devicesService,
        ILogger<WispoFcmFeedbackAdapter<TData>> logger,
        IWispoFcmMessageMapperFactory<TData> messageMapperFactory,
        IWispoFcmSender wispoFcmSender)
    {
        _next = next;
        _devicesService = devicesService;
        _logger = logger;
        _messageMapperFactory = messageMapperFactory;
        _wispoFcmSender = wispoFcmSender;
    }

    public async Task SendNewAsync(IEnumerable<Notification<TData>> notifications)
    {
        var array = notifications as Notification<TData>[] ?? notifications.ToArray();

        try
        {
            await SendPushNotifications(array);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send FCMs push notifications");
        }

        await _next.SendNewAsync(array);
    }

    private async Task SendPushNotifications(Notification<TData>[] notifications)
    {
        if (notifications.Length == 0)
            return;

        var devicesByRecipientId = await _devicesService.GetMapByRecipientIdAsync(
            notifications.Select(e => e.RecipientId));
        var mapper = await _messageMapperFactory.GetMapper(notifications);

        await _wispoFcmSender.SendAsync(EnumerateArgs());
        return;

        IEnumerable<(WispoFcmDevice, Message)> EnumerateArgs()
        {
            foreach (var group in notifications.GroupBy(e => e.RecipientId))
            {
                var devices = devicesByRecipientId[group.Key].ToArray();

                if (devices.Length == 0)
                    continue;

                foreach (var notification in group)
                {
                    foreach (var device in devices)
                    {
                        var message = mapper.Map(notification, device);
                        yield return (device, message);
                    }
                }
            }
        }
    }

    public Task SendReadAsync(IEnumerable<Notification<TData>> notifications)
    {
        return _next.SendReadAsync(notifications);
    }
}