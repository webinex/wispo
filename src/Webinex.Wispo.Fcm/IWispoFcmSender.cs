using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Webinex.Wispo.Fcm.Devices;

namespace Webinex.Wispo.Fcm;

internal interface IWispoFcmSender
{
    Task SendAsync(IEnumerable<(WispoFcmDevice Device, Message Message)> messages);
}

internal class WispoFcmSender : IWispoFcmSender
{
    private const string FIREBASE_APP_NAME = "WISPO_FCM_APP";
    private const int MAX_MESSAGES_PER_CHUNK = 100;

    private readonly Lazy<FirebaseMessaging> _messaging;
    private readonly ILogger<WispoFcmSender> _logger;

    public WispoFcmSender(WispoFcmOptions options, ILogger<WispoFcmSender> logger)
    {
        _logger = logger;
        _messaging = new Lazy<FirebaseMessaging>(() =>
        {
            var app = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(options.FcmJsonCredentialData),
            }, FIREBASE_APP_NAME);
            return FirebaseMessaging.GetMessaging(app);
        });
    }

    public async Task SendAsync(IEnumerable<(WispoFcmDevice Device, Message Message)> messages)
    {
        foreach (var chunk in messages.Chunk(MAX_MESSAGES_PER_CHUNK))
        {
            await Task.WhenAll(chunk.Select(e => SendAsync(e.Device, e.Message)));
        }
    }

    private async Task SendAsync(WispoFcmDevice device, Message message)
    {
        try
        {
            await _messaging.Value.SendAsync(message);
        }
        catch (FirebaseMessagingException e) when (e.MessagingErrorCode == MessagingErrorCode.Unregistered)
        {
            // TODO: Later we can add logic to remove such devices from database
            return;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Failed to send notification to device {DeviceTokenId}. Error message: '{ErrorMessage}'",
                device.Id,
                e.Message);
        }
    }
}