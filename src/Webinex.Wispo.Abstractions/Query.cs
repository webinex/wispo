using System.Collections.Generic;
using System.Linq;
using Webinex.Asky;

namespace Webinex.Wispo;

/// <summary>
///     Get notifications query arguments
/// </summary>
public class Query
{
    public Query(
        QueryProp props = QueryProp.Unspecified,
        FilterRule? filterRule = null,
        IEnumerable<SortRule>? sortRule = null,
        PagingRule? pagingRule = null,
        Account? onBehalfOf = null)
    {
        OnBehalfOf = onBehalfOf;
        Props = props;
        FilterRule = filterRule;
        SortRule = sortRule?.ToArray();
        PagingRule = pagingRule;
    }

    public Account? OnBehalfOf { get; }
    public QueryProp Props { get; }
    public FilterRule? FilterRule { get; }
    public SortRule[]? SortRule { get; }
    public PagingRule? PagingRule { get; }
}