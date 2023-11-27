using System;

namespace Webinex.Wispo;

public class Notification<TData> : INotificationBase
{
    public Notification(
        Guid id,
        string type,
        string recipientId,
        bool read,
        DateTimeOffset createdAt,
        DateTimeOffset? readAt,
        string? readById,
        TData data)
    {
        Id = id;
        Type = type;
        Data = data;
        RecipientId = recipientId;
        Read = read;
        CreatedAt = createdAt;
        ReadAt = readAt;
        ReadById = readById;
    }

    public Guid Id { get; }
    public string Type { get; }
    public string RecipientId { get; }
    public TData Data { get; }
    public bool Read { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ReadAt { get; }
    public string? ReadById { get; }
}