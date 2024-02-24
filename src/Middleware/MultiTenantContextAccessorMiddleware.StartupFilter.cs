using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Middleware
{
    /// <summary>
    /// Register the multitenant context accessor middleware with the app pipeline.
    /// </summary>
    /// <seealso cref="IStartupFilter" />
    internal class MultiTenantContextAccessorStartupFilter<T>() : IStartupFilter where T : ITenantInfo
    {
        /// <summary>
        /// Adds the multitenant request services middleware to the app pipeline.
        /// </summary>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<MultiTenantContextAccessorMiddleware<T>>();
                next(builder);
            };
        }
    }
}
