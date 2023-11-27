using System;

namespace Webinex.Wispo;

public interface INotificationBase
{
    Guid Id { get; }
    string Type { get; }
    bool Read { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? ReadAt { get; }
    string? ReadById { get; }
}