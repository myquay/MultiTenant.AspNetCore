using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.AspNetCore
{
    /// <summary>
    /// Provides access to the current tenant context
    /// </summary>
    public interface IMultiTenantContextAccessor<T> where T : ITenantInfo
    {
        /// <summary>
        /// Current tenant
        /// </summary>
        T? TenantInfo { get; set; }
    }
}
