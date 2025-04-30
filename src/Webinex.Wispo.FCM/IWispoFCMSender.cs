using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;

namespace Webinex.Wispo.FCM;

internal interface IWispoFCMSender
{
    Task SendAsync(IEnumerable<Message> messages);
}

internal class WispoFCMSender : IWispoFCMSender
{
    private const string FIREBASE_APP_NAME = "WISPO_FCM_APP";
    private const int MAX_MESSAGES_PER_CHUNK = 100;

    private readonly Lazy<FirebaseMessaging> _messaging;
    private readonly ILogger<WispoFCMSender> _logger;

    public WispoFCMSender(WispoFCMOptions options, ILogger<WispoFCMSender> logger)
    {
        _logger = logger;
        _messaging = new Lazy<FirebaseMessaging>(() =>
        {
            var app = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(options.JsonCredentialData),
            }, FIREBASE_APP_NAME);
            return FirebaseMessaging.GetMessaging(app);
        });
    }

    public async Task SendAsync(IEnumerable<Message> messages)
    {
        foreach (var chunk in messages.Chunk(MAX_MESSAGES_PER_CHUNK))
        {
            await Task.WhenAll(chunk.Select(e => SendAsync(e)));
        }
    }

    private async Task SendAsync(Message message)
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
                "Failed to send notification to device token {DeviceToken}. Error message: '{ErrorMessage}'",
                message.Token,
                e.Message);
        }
    }
}