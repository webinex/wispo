using System;

namespace Webinex.Wispo.DataAccess;

public class NotificationRow<TData>
{
    public Guid Id { get; protected set; }
    public string RecipientId { get; protected set; } = null!;
    public string Type { get; protected set; } = null!;
    public TData Data { get; protected set; } = default!;
    public bool IsRead { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public string? ReadById { get; protected set; } = null!;
    public DateTimeOffset? ReadAt { get; protected set; }

    protected NotificationRow()
    {
    }

    internal static NotificationRow<TData> New(
        string type,
        string recipientId,
        TData data,
        DateTimeOffset createdAt)
    {
        return new NotificationRow<TData>
        {
            Id = Guid.NewGuid(),
            RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId)),
            Data = data,
            IsRead = false,
            CreatedAt = createdAt,
            Type = type ?? throw new ArgumentNullException(nameof(type)),
        };
    }

    public void Read(string readById)
    {
        readById = readById ?? throw new ArgumentNullException(nameof(readById));

        IsRead = true;
        ReadById = readById;
        ReadAt = DateTimeOffset.UtcNow;
    }

    internal Notification<TData> ToNotification()
    {
        return new Notification<TData>(Id, Type, RecipientId, IsRead, CreatedAt, ReadAt, ReadById, Data);
    }
}