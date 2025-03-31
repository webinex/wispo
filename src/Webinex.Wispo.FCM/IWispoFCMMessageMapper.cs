using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Webinex.Wispo.FCM.Devices;

namespace Webinex.Wispo.FCM;

public interface IWispoFCMMessageMapper<TData>
{
    Task<IEnumerable<Message>> MapAsync(
        IEnumerable<(WispoFCMDevice Device, Notification<TData> Notification)> notifications);
}

public class WispoFCMDictMappedMessage
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

internal class DictionaryWispoFCMMessageMapper<TData> : IWispoFCMMessageMapper<TData>
{
    private readonly IReadOnlyDictionary<string, WispoFCMDictMappedMessage> _dict;
    private readonly Func<string, WispoFCMDictMappedMessage> _fallback;

    public DictionaryWispoFCMMessageMapper(
        IReadOnlyDictionary<string, WispoFCMDictMappedMessage> dict,
        Func<string, WispoFCMDictMappedMessage> fallback)
    {
        _dict = dict;
        _fallback = fallback;
    }

    public Task<IEnumerable<Message>> MapAsync(
        IEnumerable<(WispoFCMDevice Device, Notification<TData> Notification)> notifications)
    {
        var result = notifications.Select(e =>
        {
            if (!_dict.TryGetValue(e.Notification.Type, out var message))
                message = _fallback(e.Notification.Type);

            return new Message
            {
                Token = e.Device.Token,
                Notification = new Notification
                {
                    Title = message.Title,
                    Body = message.Body,
                },
            };
        });

        return Task.FromResult(result);
    }
}

internal class DefaultWispoFCMMessageMapper<TData> : IWispoFCMMessageMapper<TData>
{
    public Task<IEnumerable<Message>> MapAsync(
        IEnumerable<(WispoFCMDevice Device, Notification<TData> Notification)> notifications)
    {
        var result = notifications.Select(e => new Message
        {
            Token = e.Device.Token,
            Notification = new Notification
            {
                Title = $"Wispo example '{e.Notification.Type}' notification",
                Body = $"Consider overriding IWispoFCMMessageMapper<TData> to customize FCM messages",
            },
            Webpush = new WebpushConfig
            {
                FcmOptions = new WebpushFcmOptions()
                {
                    Link = "https://webinex.github.io/wispo/docs/getting-started",
                },
            },
        });

        return Task.FromResult(result);
    }
}