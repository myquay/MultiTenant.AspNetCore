using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure
{
    /// <summary>
    /// Method to register our middleware
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Use the Tenant Middleware to process the request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMultiTenancy<T>(this IApplicationBuilder builder) where T : ITenantInfo
            => builder.UseMiddleware<TenantMiddleware<T>>();

    }
}
