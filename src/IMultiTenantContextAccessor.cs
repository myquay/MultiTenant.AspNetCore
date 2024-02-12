using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Provides access to the current tenant context
    /// </summary>
    public interface IMultiTenantContextAccessor
    {
        /// <summary>
        /// Current tenant Id
        /// </summary>
        string? TenantId { get; set;  }
    }
}
