using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Services
{
    internal class InMemoryLookupService<T>(IEnumerable<T> Tenants) : ITenantLookupService<T> where T : ITenantInfo
    {
        public Task<T> GetTenantAsync(string identifier)
        {
            return Task.FromResult(Tenants.Single(t => t.Identifier == identifier));
        }
    }
}
