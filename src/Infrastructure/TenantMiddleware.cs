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
    internal class TenantMiddleware
    {
        private readonly RequestDelegate next;

        public TenantMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var accessor = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor>();

            if (accessor.TenantId == null)
            {
                var resolver = context.RequestServices.GetRequiredService<ITenantContextService>();
                accessor.TenantId = await resolver.GetTenantIdAsync();
            }

            var provider = context.RequestServices.GetRequiredService<IMultiTenantServiceProvider>();
            context.RequestServices = provider;

            //Continue processing
            if (next != null)
                await next(context);
        }
    }
}
