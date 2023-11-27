using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Wispo;

public class Account
{
    public Account(string id, IReadOnlyCollection<string>? groups = null)
    {
        Id = id;
        Groups = groups ?? Array.Empty<string>();
    }

    public string Id { get; }
    public IReadOnlyCollection<string> Groups { get; }
    public IReadOnlyCollection<string> All => new[] { Id }.Concat(Groups).ToArray();
}