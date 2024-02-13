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
    internal class TenantMiddleware<T> where T : ITenantInfo
    {
        private readonly RequestDelegate next;

        public TenantMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var accessor = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor<T>>();

            if (accessor.TenantInfo == null)
            {
                var resolver = context.RequestServices.GetRequiredService<ITenantLookupService<T>>();
                var tenantIdentifierResolver = context.RequestServices.GetRequiredService<ITenantResolutionStrategy>();

                accessor.TenantInfo = await resolver.GetTenantAsync(await tenantIdentifierResolver.GetTenantIdentifierAsync());
            }

            var provider = context.RequestServices.GetRequiredService<IMultiTenantServiceProvider>();
            context.RequestServices = provider;

            //Continue processing
            if (next != null)
                await next(context);
        }
    }
}
