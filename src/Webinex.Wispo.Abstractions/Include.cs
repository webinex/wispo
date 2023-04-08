using System;

namespace Webinex.Wispo
{
    [Flags]
    public enum Include
    {
        Unspecified = 0,
        TotalUnread = 1,
        Total = 2,
        TotalMatch = 4,
        Items = 8,
    }
}