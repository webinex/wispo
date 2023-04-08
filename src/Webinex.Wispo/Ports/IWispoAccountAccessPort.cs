using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Webinex.Wispo.Ports
{
    public interface IWispoAccountAccessPort
    {
        Task<IDictionary<string, string[]>> GetAccountsWhichHasAccessToAccount(IEnumerable<string> accountIds);

        Task<IDictionary<string, string[]>> GetAccountsAccountHasAccess(IEnumerable<string> accountIds);
    }

    internal class DefaultWispoAccountAccessAdapter : IWispoAccountAccessPort
    {
        public Task<IDictionary<string, string[]>> GetAccountsWhichHasAccessToAccount(IEnumerable<string> accountIds)
        {
            var result = accountIds.ToDictionary(id => id, id => new[] { id });
            return Task.FromResult<IDictionary<string, string[]>>(result);
        }

        public Task<IDictionary<string, string[]>> GetAccountsAccountHasAccess(IEnumerable<string> accountIds)
        {
            var result = accountIds.ToDictionary(id => id, id => new[] { id });
            return Task.FromResult<IDictionary<string, string[]>>(result);
        }
    }

    internal static class WispoAccountAccessPortExtensions
    {
        public static async Task<string[]> GetAccountsAccountHasAccess(
            [NotNull] this IWispoAccountAccessPort accountAccessPort,
            [NotNull] string accountId)
        {
            accountAccessPort = accountAccessPort ?? throw new ArgumentNullException(nameof(accountAccessPort));
            accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));

            var result = await accountAccessPort.GetAccountsAccountHasAccess(new[] { accountId });
            return result.Values.Single().ToArray();
        }
    }
}