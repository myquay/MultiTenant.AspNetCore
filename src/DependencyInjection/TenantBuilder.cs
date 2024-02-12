using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Contrib.MultiTenant.Services;
using Microsoft.AspNetCore.Contrib.MultiTenant.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection
{
    /// <summary>
    /// Tenant builder
    /// </summary>
    /// <param name="services"></param>
    public class TenantBuilder(WebApplicationBuilder builder)
    {
        /// <summary>
        /// Register the tenant resolver implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder WithResolutionStrategy<V>() where V : class, ITenantResolutionStrategy
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped(typeof(ITenantResolutionStrategy), typeof(V));
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
            builder.Services.AddScoped(typeof(ITenantLookupService), typeof(V));
            return this;
        }

        /// <summary>
        /// Register the tenant in memory lookup service implementation
        /// </summary>
        /// <param name="tenants"></param>
        /// <returns></returns>
        public TenantBuilder WithInMemoryTenantMapping((string Id, string Identifier)[] tenants)
        {
            builder.Services.AddSingleton<ITenantLookupService>(new InMemoryLookupService(tenants));
            return this;
        }

        public TenantBuilder WithTenantedServices(Action<string, IServiceCollection> configuration)
        {

            builder.Services.RemoveAll<IServiceProvider>();
            builder.Services.AddSingleton<IMultiTenantServiceProvider>(new MultiTenantServiceProvider(builder.Services, configuration));
            builder.Host.UseServiceProviderFactory(context => new MultiTenantServiceProviderFactory(configuration));

            return this;
        }

    }
}
