using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Returns the tenant id for a specific identifier
    /// </summary>
    public interface ITenantLookupService
    {
        /// <summary>
        /// Given an identifier, it returns the durable tenant id
        /// </summary>
        /// <returns></returns>
        Task<string> GetTenantIdAsync(string identifier);
    }
}
