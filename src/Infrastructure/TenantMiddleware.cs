using Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure
{
    /// <summary>
    /// Multi-tenant middleware
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="TenantAccessor"></param>
    /// <param name="TenantResolver"></param>
    /// <param name="TenantResolutionStrategy"></param>
    public class TenantMiddleware<T>(IMultiTenantContextAccessor<T> TenantAccessor, ITenantLookupService<T> TenantResolver, ITenantResolutionStrategy TenantResolutionStrategy) : IMiddleware where T : ITenantInfo
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //Set the tenant context
            TenantAccessor.TenantInfo ??= await TenantResolver.GetTenantAsync(await TenantResolutionStrategy.GetTenantIdentifierAsync());

            //Configure the service provider
            var provider = context.RequestServices.GetRequiredService<IMultiTenantServiceProvider>();
            context.RequestServices = provider;

            //Continue processing
            if (next != null)
                await next(context);
        }
    }
}
