using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Basic tenant info
    /// </summary>
    public interface ITenantInfo
    {
        /// <summary>
        /// Tenant id
        //// </summary>
        string Id { get; set; }
                        
        /// <summary>
        /// Tenant name
        /// </summary>
        string Identifier { get; set; }
    }
}
