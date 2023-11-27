using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Wispo;

public class ReadArgs
{
    public ReadArgs(IEnumerable<Guid> id, Account onBehalfOf)
    {
        Id = id.ToArray();
        OnBehalfOf = onBehalfOf;
    }

    public Account OnBehalfOf { get; }
    public IReadOnlyCollection<Guid> Id { get; }
}