using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webinex.Asky;

namespace Webinex.Wispo.AspNetCore;

public interface IWispoAspNetCore
{
    Task<IActionResult> QueryAsync(
        FilterRule? filterRule,
        SortRule[]? sortRule,
        PagingRule? pagingRule,
        QueryProp queryProp);

    Task<IActionResult> GetAsync(Guid id);
    Task<IActionResult> TotalUnreadCountAsync();
    Task<IActionResult> ReadAsync(Guid[] ids);
    Task<IActionResult> ReadAllAsync();
}

internal class WispoAspNetCore<TData> : IWispoAspNetCore
{
    private readonly IWispoAspNetCoreContextService _contextService;
    private readonly IWispo<TData> _wispo;
    private readonly IWispoAspNetCoreMapper<TData> _mapper;

    public WispoAspNetCore(
        IWispoAspNetCoreContextService contextService,
        IWispo<TData> wispo,
        IWispoAspNetCoreMapper<TData> mapper)
    {
        _contextService = contextService;
        _wispo = wispo;
        _mapper = mapper;
    }

    public async Task<IActionResult> QueryAsync(
        FilterRule? filterRule,
        SortRule[]? sortRule,
        PagingRule? pagingRule,
        QueryProp queryProp)
    {
        var query = new Query(
            props: queryProp,
            filterRule: filterRule,
            sortRule: sortRule,
            pagingRule: pagingRule,
            onBehalfOf: await _contextService.GetAsync());
        
        var result = await _wispo.QueryAsync(query);
        var items = result.Items != null ? await _mapper.MapAsync(result.Items) : null;
        
        return new OkObjectResult(new QueryResultDto(items, result.TotalUnreadCount, result.TotalCount, result.TotalMatchCount));
    }

    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _wispo.GetAsync(id, onBehalfOf: await _contextService.GetAsync());
        var mapped = (await _mapper.MapAsync(new[] { result })).Single();
        return new OkObjectResult(mapped);
    }

    public async Task<IActionResult> TotalUnreadCountAsync()
    {
        var result = await _wispo.TotalUnreadCountAsync(onBehalfOf: await _contextService.GetAsync());
        return new OkObjectResult(result);
    }

    public async Task<IActionResult> ReadAsync(Guid[] ids)
    {
        await _wispo.ReadAsync(ids: ids, onBehalfOf: await _contextService.GetAsync());
        return new OkResult();
    }

    public async Task<IActionResult> ReadAllAsync()
    {
        await _wispo.ReadAllAsync(onBehalfOf: await _contextService.GetAsync());
        return new OkResult();
    }
}