using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure
{
    internal class TenantContextService(ITenantResolutionStrategy tenantResolutionStrategy, ITenantLookupService tenantLookupService) : ITenantContextService
    {
        private readonly ITenantResolutionStrategy _tenantResolutionStrategy = tenantResolutionStrategy;
        private readonly ITenantLookupService _tenantLookupService = tenantLookupService;

        public async Task<string> GetTenantIdAsync()
        {
            var identifier = await _tenantResolutionStrategy.GetTenantIdentifierAsync();
            return await _tenantLookupService.GetTenantIdAsync(identifier);
        }
    }
}
