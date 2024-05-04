using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.AspNetCore
{
    /// <summary>
    /// Configuration options
    /// </summary>
    public class MultiTenantOptions
    {
        /// <summary>
        /// Disable the automatic registration of the multitenant pipeline
        /// </summary>
        public bool DisableAutomaticPipelineRegistration { get; set; } = false;
    }
}
