using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webinex.Wispo.AspNetCore
{
    internal class DefaultUserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<string> GetAsync()
        {
            var id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null)
                throw new InvalidOperationException($"Unable to find claim {ClaimTypes.NameIdentifier} in http context user claims.");

            return Task.FromResult(id);
        }
    }
}