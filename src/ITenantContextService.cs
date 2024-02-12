using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Resolves the current tenant Id
    /// </summary>
    public interface ITenantContextService
    {
        /// <summary>
        /// Get the current tenant id
        /// </summary>
        /// <returns></returns>
        public Task<string> GetTenantIdAsync();
    }
}
