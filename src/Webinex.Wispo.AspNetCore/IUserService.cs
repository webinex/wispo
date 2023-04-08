using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Webinex.Wispo.AspNetCore
{
    /// <summary>
    ///     Accessor for current user recipient identifier.
    ///     Default implementation uses <see cref="IHttpContextAccessor"/> and gets value of NameIdentifier claim
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///     Returns current user recipient identifier
        /// </summary>
        /// <returns>Current user recipient identifier</returns>
        Task<string> GetAsync();
    }
}