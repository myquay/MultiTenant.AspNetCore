using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Services
{
    /// <summary>
    /// Simple in-memory tenant mapping service
    /// </summary>
    /// <param name="TenantMappings"></param>
    public record InMemoryLookupService((String Id, String Identifier)[] TenantMappings) : ITenantLookupService
    {
        /// <summary>
        /// Given an identifier, it returns the durable tenant id
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task<string> GetTenantIdAsync(string identifier)
        {
            if(!TenantMappings.Any(x => x.Identifier == identifier))
                throw new InvalidOperationException($"Tenant not found for identifier {identifier}");

            if(TenantMappings.Count(x => x.Identifier == identifier) > 1)
                throw new InvalidOperationException($"Multiple tenants found for identifier {identifier}");

            return Task.FromResult(TenantMappings.Single(x => x.Identifier == identifier).Id);
        }
    }
}
