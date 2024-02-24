using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using Microsoft.AspNetCore.Contrib.MultiTenant.Middleware;
using Microsoft.AspNetCore.Contrib.MultiTenant.Services;
using Microsoft.AspNetCore.Contrib.MultiTenant.Strategies;
using Microsoft.AspNetCore.Hosting;
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
    public class TenantBuilder<T>(WebApplicationBuilder builder) where T : ITenantInfo
    {
        /// <summary>
        /// Register the tenant resolver implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithResolutionStrategy<V>() where V : class, ITenantResolutionStrategy
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped(typeof(ITenantResolutionStrategy), typeof(V));
            return this;
        }

        /// <summary>
        /// Helper for host resolution strategy
        /// </summary>
        /// <returns></returns>
        public TenantBuilder<T> WithHostResolutionStrategy()
        {
            return WithResolutionStrategy<HostResolutionStrategy>();
        }

        /// <summary>
        /// Register the tenant lookup service implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithTenantLookupService<V>() where V : class, ITenantLookupService<T>
        {
            builder.Services.AddScoped<ITenantLookupService<T>, V>();
            return this;
        }

        /// <summary>
        /// Register tenant specific services
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithTenantedServices(Action<T, IServiceCollection> configuration)
        {
            //Replace the default service provider with a multitenant service provider
            builder.Services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultitenantRequestServicesStartupFilter<T>()));

            //Register the multi-tenant service provider
            builder.Services.AddSingleton(new MultiTenantServiceProviderFactory<T>(builder.Services, configuration));
            builder.Services.AddSingleton<IMultiTenantServiceScopeFactory, MultiTenantServiceScopeFactory<T>>();

            //Some services might be registered before the tenant middleware, so not available during the application startup
            builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);

            return this;
        }

    }
}
