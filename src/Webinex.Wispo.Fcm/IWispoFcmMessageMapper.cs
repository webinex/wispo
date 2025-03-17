using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Webinex.Wispo.Fcm.Devices;

namespace Webinex.Wispo.Fcm;

public interface IWispoFcmMessageMapper<TData>
{
    Message Map(Notification<TData> notification, WispoFcmDevice device);
}

public interface IWispoFcmMessageMapperFactory<TData>
{
    Task<IWispoFcmMessageMapper<TData>> GetMapper(Notification<TData>[] notifications);
}

internal class DefaultWispoFcmMessageMapperFactory<TData> : IWispoFcmMessageMapperFactory<TData>
{
    public Task<IWispoFcmMessageMapper<TData>> GetMapper(Notification<TData>[] notifications)
    {
        return Task.FromResult<IWispoFcmMessageMapper<TData>>(new Mapper());
    }

    private class Mapper : IWispoFcmMessageMapper<TData>
    {
        public Message Map(Notification<TData> notification, WispoFcmDevice device)
        {
            return new Message
            {
                Token = device.Token,
                Notification = new Notification
                {
                    Title = $"Wispo example '{notification.Type}' notification",
                    Body = $"Consider overriding IWispoFcmMessageMapperFactory<TData> to customize FCM messages",
                },
                Webpush = new WebpushConfig
                {
                    FcmOptions = new WebpushFcmOptions
                    {
                        Link = "https://webinex.github.io/wispo/docs/getting-started",
                    },
                },
            };
        }
    }
}