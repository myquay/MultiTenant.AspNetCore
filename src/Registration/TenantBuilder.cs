using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MultiTenant.AspNetCore.Infrastructure.DependencyInjection;
using MultiTenant.AspNetCore.Infrastructure.Middleware;
using MultiTenant.AspNetCore.Infrastructure.Options;
using MultiTenant.AspNetCore.Infrastructure.Strategies;
using MultiTenant.AspNetCore.Services;

namespace MultiTenant.AspNetCore.Builder
{
    /// <summary>
    /// Tenant builder
    /// </summary>
    /// <param name="services"></param>
    public class TenantBuilder<T>(IServiceCollection Services, MultiTenantOptions options) where T : ITenantInfo
    {
        /// <summary>
        /// Register the tenant resolver implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithResolutionStrategy<V>() where V : class, ITenantResolutionStrategy
        {
            Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.TryAddSingleton(typeof(ITenantResolutionStrategy), typeof(V));
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
            Services.TryAddSingleton<ITenantLookupService<T>, V>();
            return this;
        }

        /// <summary>
        /// Register the tenant lookup service implementation
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithInMemoryTenantLookupService(IEnumerable<T> tenants)
        {
            var service = new InMemoryLookupService<T>(tenants);
            Services.TryAddSingleton<ITenantLookupService<T>>(service);
            return this;
        }


        /// <summary>
        /// Register tenant specific services
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithTenantedServices(Action<IServiceCollection, T?> configuration)
        {
            //Replace the default service provider with a multitenant service provider
            if(!options.DisableAutomaticPipelineRegistration)
                Services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultitenantRequestServicesStartupFilter<T>()));

            //Register the multi-tenant service provider
            Services.AddSingleton(new MultiTenantServiceProviderFactory<T>(Services, configuration));
            Services.AddSingleton<IMultiTenantServiceScopeFactory, MultiTenantServiceScopeFactory<T>>();

            return this;
        }

        /// <summary>
        /// Configure tenant specific options
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="tenantOptionsConfiguration"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithTenantedConfigure<TOptions>(Action<TOptions, T?> tenantOptionsConfiguration) where TOptions : class
        {
            Services.AddOptions();

            Services.TryAddSingleton<IOptionsMonitorCache<TOptions>, MultiTenantOptionsCache<TOptions, T>>();
            Services.TryAddScoped<IOptionsSnapshot<TOptions>>((sp) =>
            {
                return new MultiTenantOptionsManager<TOptions>(sp.GetRequiredService<IOptionsFactory<TOptions>>(), sp.GetRequiredService<IOptionsMonitorCache<TOptions>>());
            });
            Services.TryAddSingleton<IOptions<TOptions>>((sp) =>
            {
                return new MultiTenantOptionsManager<TOptions>(sp.GetRequiredService<IOptionsFactory<TOptions>>(), sp.GetRequiredService<IOptionsMonitorCache<TOptions>>());
            });

            Services.AddSingleton<IConfigureOptions<TOptions>, ConfigureOptions<TOptions>>((IServiceProvider sp) =>
            {
                var tenantAccessor = sp.GetRequiredService<IMultiTenantContextAccessor<T>>();
                return new ConfigureOptions<TOptions>((options) => tenantOptionsConfiguration(options, tenantAccessor.TenantInfo));

            });

            return this;
        }

    }
}
