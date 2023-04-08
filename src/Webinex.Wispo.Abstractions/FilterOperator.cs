using System.Collections.Generic;
using System.Linq;

namespace Webinex.Wispo
{
    public static class FilterOperator
    {
        public const string EQ = "=";
        public const string NOT_EQ = "!=";
        public const string GTE = ">=";
        public const string GT = ">";
        public const string LTE = "<=";
        public const string LT = "<";
        public const string CONTAINS = "contains";
        public const string NOT_CONTAINS = "!contains";
        public const string AND = "and";
        public const string OR = "or";
        public const string IN = "in";
        public const string NOT_IN = "!in";

        public static readonly HashSet<string> ALL_VALUES = new HashSet<string>
        {
            EQ, NOT_EQ, GTE, GT, LTE, LT, CONTAINS, NOT_CONTAINS,
        };

        public static readonly HashSet<string> ALL_LOGICAL = new HashSet<string>
        {
            AND, OR,
        };

        public static readonly HashSet<string> ALL_IN = new HashSet<string>
        {
            IN, NOT_IN,
        };

        public static readonly HashSet<string> ALL = ALL_VALUES.Concat(ALL_LOGICAL).Concat(ALL_IN).ToHashSet();
    }
}