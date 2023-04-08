using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Wispo.AspNetCore
{
    [Route("/api/wispo")]
    public class WispoController : ControllerBase
    {
        private readonly IWispo _wispo;
        private readonly IUserService _userService;
        private readonly IWispoControllerSettings _settings;

        public WispoController(IWispo wispo, IUserService userService, IWispoControllerSettings settings)
        {
            _wispo = wispo;
            _userService = userService;
            _settings = settings;
        }

        private IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        
        private IAuthenticationService AuthenticationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int? skip,
            [FromQuery] int? take,
            [FromQuery(Name = "include")] string includeString,
            [FromQuery(Name = "filter")] string filterString,
            [FromQuery(Name = "sort")] string sortString)
        {
            if (!await AuthorizeAsync())
                return Forbid();
            
            var userId = await UserIdAsync();
            var filter = FilterDto.FromJson(filterString)?.ToFilter();
            var include = MapInclude(includeString);
            var sort = string.IsNullOrWhiteSpace(sortString)
                ? null
                : JsonSerializer.Deserialize<SortRule[]>(sortString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() },
                });

            if (sort?.Any() != true)
            {
                sort = new[] { SortRule.Desc(NotificationField.CREATED_AT) };
            }

            var args = new GetNotificationArgs(userId)
            {
                Skip = skip,
                Take = take,
                Include = include,
                Filter = filter,
                Sort = sort,
            };

            var result = await _wispo.GetAsync(args);
            return Ok(result);
        }

        private Include MapInclude(string includeString)
        {
            if (string.IsNullOrWhiteSpace(includeString))
                return Include.Unspecified;

            return includeString
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Enum.Parse<Include>(x, true))
                .Aggregate(Include.Unspecified, (result, right) => result | right);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            var userId = await UserIdAsync();
            var result = await _wispo.GetAsync(userId, id);
            return Ok(result);
        }

        [HttpGet("unread/ids")]
        public async Task<IActionResult> GetUnreadIdsAsync()
        {
            if (!await AuthorizeAsync())
                return Forbid();

            var userId = await UserIdAsync();
            var result = await _wispo.GetUnreadIdsAsync(userId);
            return Ok(result);
        }

        [HttpPut("read")]
        public async Task<IActionResult> MarkReadAsync([FromBody] Guid[] id)
        {
            if (!await AuthorizeAsync())
                return Forbid();
            
            var userId = await UserIdAsync();
            await _wispo.MarkReadAsync(userId, id);
            return Ok();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllReadAsync()
        {
            if (!await AuthorizeAsync())
                return Forbid();
            
            var userId = await UserIdAsync();
            await _wispo.MarkAllReadAsync(userId);
            return Ok();
        }

        private async Task<string> UserIdAsync()
        {
            var userId = await _userService.GetAsync();
            return userId ?? throw new InvalidOperationException("UserId could not be null.");
        }

        private async Task<bool> AuthorizeAsync()
        {
            if (_settings.Policy == null)
                return true;

            var authenticationResult = await AuthenticationService.AuthenticateAsync(HttpContext, _settings.Schema);
            if (!authenticationResult.Succeeded) 
                return false;
            
            var authorizationResult = await AuthorizationService.AuthorizeAsync(authenticationResult.Principal!, _settings.Policy);
            return authorizationResult.Succeeded;
        }
    }
}