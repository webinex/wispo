using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Webinex.Wispo.FCM.Devices;
using Webinex.Wispo.Ports;

namespace Webinex.Wispo.FCM;

internal class WispoFCMFeedbackAdapter<TData> : IWispoFeedbackPort<TData>
{
    private readonly IWispoFeedbackPort<TData> _next;
    private readonly IWispoFCMDevicesService _devicesService;
    private readonly ILogger _logger;
    private readonly IWispoFCMMessagesMapper<TData> _messagesMapper;
    private readonly IWispoFCMSender _wispoFCMSender;

    public WispoFCMFeedbackAdapter(
        IWispoFeedbackPort<TData> next,
        IWispoFCMDevicesService devicesService,
        ILogger<WispoFCMFeedbackAdapter<TData>> logger,
        IWispoFCMMessagesMapper<TData> messagesMapper,
        IWispoFCMSender wispoFCMSender)
    {
        _next = next;
        _devicesService = devicesService;
        _logger = logger;
        _messagesMapper = messagesMapper;
        _wispoFCMSender = wispoFCMSender;
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

        var mapped = await _messagesMapper.MapAsync(EnumerateArgs());
        await _wispoFCMSender.SendAsync(mapped);
        return;

        IEnumerable<(WispoFCMDevice, Notification<TData>)> EnumerateArgs()
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
                        yield return (device, notification);
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