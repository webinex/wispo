using System;

namespace Webinex.Wispo;

public class AddArgs<TData>
{
    public AddArgs(string type, string recipientId, TData data)
    {
        RecipientId = recipientId ?? throw new ArgumentNullException(nameof(recipientId));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public string Type { get; }
    public string RecipientId { get; }
    public TData Data { get; }
}