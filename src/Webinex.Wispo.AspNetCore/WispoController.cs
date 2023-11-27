using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Webinex.Asky;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.AspNetCore;

[Route("/api/wispo")]
public class WispoController<TData> : ControllerBase
{
    private readonly IWispoAspNetCore _wispo;
    private readonly IAskyFieldMap<NotificationRow<TData>> _askyFieldMap;
    private readonly IOptions<JsonOptions> _jsonOptions;

    public WispoController(
        IWispoAspNetCore wispo,
        IAskyFieldMap<NotificationRow<TData>> askyFieldMap,
        IOptions<JsonOptions> jsonOptions)
    {
        _wispo = wispo;
        _askyFieldMap = askyFieldMap;
        _jsonOptions = jsonOptions;
    }

    [HttpGet]
    [WispoAuthorization]
    public async Task<IActionResult> QueryAsync(
        [FromQuery(Name = "filterRule")] string? filterString,
        [FromQuery(Name = "sortRule")] string? sortString,
        [FromQuery(Name = "pagingRule")] string? pagingString,
        [FromQuery(Name = "props")] string? propsString)
    {
        var sortRule = SortRule.FromJsonArray(sortString, _jsonOptions.Value.JsonSerializerOptions);
        sortRule = sortRule?.Any() == true ? sortRule : new[] { SortRule.Desc("createdAt") };

        return await _wispo.QueryAsync(
            filterRule: FilterRule.FromJson(filterString, _askyFieldMap),
            sortRule: sortRule,
            pagingRule: PagingRule.FromJson(pagingString, _jsonOptions.Value.JsonSerializerOptions),
            queryProp: ParseProps(propsString));
    }

    private QueryProp ParseProps(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return QueryProp.Unspecified;

        return value
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => Enum.Parse<QueryProp>(x, true))
            .Aggregate(QueryProp.Unspecified, (result, right) => result | right);
    }

    [HttpGet("{id}")]
    [WispoAuthorization]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        return await _wispo.GetAsync(id);
    }

    [HttpGet("unread/count")]
    [WispoAuthorization]
    public async Task<IActionResult> TotalUnreadCountAsync()
    {
        return await _wispo.TotalUnreadCountAsync();
    }

    [HttpPut("read")]
    [WispoAuthorization]
    public async Task<IActionResult> ReadAsync([FromBody] Guid[] ids)
    {
        return await _wispo.ReadAsync(ids);
    }

    [HttpPut("read-all")]
    [WispoAuthorization]
    public async Task<IActionResult> ReadAllAsync()
    {
        return await _wispo.ReadAllAsync();
    }
}