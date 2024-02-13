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

        public TenantBuilder<T> WithTenantedServices(Action<T, IServiceCollection> configuration)
        {

            builder.Services.RemoveAll<IServiceProvider>();
            builder.Services.AddSingleton<IMultiTenantServiceProvider>(new MultiTenantServiceProvider<T>(builder.Services, configuration));
            builder.Host.UseServiceProviderFactory(context => new MultiTenantServiceProviderFactory<T>(configuration));

            return this;
        }

    }
}
