using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using Microsoft.AspNetCore.Contrib.MultiTenant;
using System.Collections.Concurrent;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.MultiTenantPipeline
{


    /// <summary>
    /// Middleware to set up the tenant specific mini-pipeline and run it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="next"></param>
    /// <param name="configurePipeline"></param>
    internal class MultiTenantMiddleware<T>(RequestDelegate next, IApplicationBuilder builder, Action<T, IApplicationBuilder> configurePipeline)
        where T : class, ITenantInfo
    {
        //Cache compiled pipelines
        private readonly ConcurrentDictionary<string, Lazy<RequestDelegate>> _pipelinesCache = new();

        /// <summary>
        /// Set the services for the tenant to be our specific tenant services
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var tenant = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor<T>>().TenantInfo;
            if (tenant == null)
            {
                await next(context);
            }
            else
            {
                var request = _pipelinesCache.GetOrAdd(tenant.Id, (key) => new Lazy<RequestDelegate>(() =>
                {
                    var nestedPipeine = builder.New();
                    configurePipeline(tenant, nestedPipeine);
                    return nestedPipeine.Use(_ => next).Build();
                })).Value;

                await request(context);
            }
        }
    }
}
