﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace MultiTenant.AspNetCore.Infrastructure.Middleware
{


    /// <summary>
    /// Middleware to set up the tenant specific mini-pipeline and run it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="next"></param>
    /// <param name="configurePipeline"></param>
    internal class MultiTenantMiddleware<T>(RequestDelegate next, IApplicationBuilder builder, Action<T, IApplicationBuilder> configurePipeline)
        where T : ITenantInfo
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
