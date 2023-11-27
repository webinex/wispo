using System;

namespace Webinex.Wispo;

[Flags]
public enum QueryProp
{
    Unspecified = 0,
    TotalUnreadCount = 1,
    TotalCount = 2,
    TotalMatchCount = 4,
    Items = 8,
}