using Microsoft.AspNetCore.Contrib.MultiTenant.Services;
using Microsoft.AspNetCore.Contrib.MultiTenant.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure
{
    /// <summary>
    /// Tenant builder
    /// </summary>
    /// <param name="services"></param>
    public class TenantBuilder(IServiceCollection services)
    {
        /// <summary>
        /// Register the tenant resolver implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder WithResolutionStrategy<V>() where V : class, ITenantResolutionStrategy
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(ITenantResolutionStrategy), typeof(V));
            return this;
        }

        /// <summary>
        /// Helper for host resolution strategy
        /// </summary>
        /// <returns></returns>
        public TenantBuilder WithHostResolutionStrategy()
        {
            return WithResolutionStrategy<HostResolutionStrategy>();
        }

        /// <summary>
        /// Register the tenant lookup service implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder TenantMappingProvider<V>() where V : class, ITenantLookupService
        {
            services.AddScoped(typeof(ITenantLookupService), typeof(V));
            return this;
        }

        /// <summary>
        /// Register the tenant in memory lookup service implementation
        /// </summary>
        /// <param name="tenants"></param>
        /// <returns></returns>
        public TenantBuilder WithInMemoryTenantMapping((String Id, String Identifier)[] tenants)
        {
            services.AddSingleton<ITenantLookupService>(new InMemoryLookupService(tenants));
            return this;
        }

    }
}
