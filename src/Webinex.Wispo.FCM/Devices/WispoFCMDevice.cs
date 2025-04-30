using System;

namespace Webinex.Wispo.FCM.Devices;

public class WispoFCMDevice
{
    public Guid Id { get; protected init; }
    public string Token { get; protected init; } = null!;
    public string RecipientId { get; protected set; } = null!;
    public string? Meta { get; protected set; }
    public DateTimeOffset UpdatedAt { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected init; }

    protected WispoFCMDevice()
    {
    }

    public WispoFCMDevice(
        Guid id,
        string token,
        string userId,
        string? meta,
        DateTimeOffset updatedAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        Token = token ?? throw new ArgumentNullException(nameof(token));
        RecipientId = userId ?? throw new ArgumentNullException(nameof(userId));
        Meta = meta;
        UpdatedAt = updatedAt;
        CreatedAt = createdAt;
    }

    public void Update(string recipientId, string? meta)
    {
        RecipientId = recipientId;
        Meta = meta;
        UpdateTimestamp();
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}