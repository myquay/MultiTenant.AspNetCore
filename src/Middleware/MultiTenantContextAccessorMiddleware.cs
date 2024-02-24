﻿using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Middleware
{
    /// <summary>
    /// This middleware is responsible for setting up the scope for the tenant specific request services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tenantServicesConfiguration"></param>
    internal class MultiTenantContextAccessorMiddleware<T>(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IMultiTenantContextAccessor<T> TenantAccessor, ITenantLookupService<T> TenantResolver, ITenantResolutionStrategy TenantResolutionStrategy) where T : ITenantInfo
    {

        /// <summary>
        /// Set the services for the tenant to be our specific tenant services
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            //Set context if missing so it can be used by the tenant services to resolve the tenant
            httpContextAccessor.HttpContext ??= context;
            TenantAccessor.TenantInfo ??= await TenantResolver.GetTenantAsync(await TenantResolutionStrategy.GetTenantIdentifierAsync());
            await next.Invoke(context);
        }
    }
}
