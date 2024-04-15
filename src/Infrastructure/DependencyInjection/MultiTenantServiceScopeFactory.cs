using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace MultiTenant.AspNetCore.Infrastructure.DependencyInjection
{

    /// <summary>
    /// Factory for creating tenant specific service providers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MultiTenantServiceProviderFactory<T>(IServiceCollection containerBuilder, Action<IServiceCollection, T?> tenantServiceConfiguration) where T : ITenantInfo
    {

        //Cache compiled providers
        private readonly ConcurrentDictionary<string, Lazy<IServiceProvider>> CompiledProviders = new();

        public IServiceProvider GetServiceProviderForTenant(T tenant)
        {
            return CompiledProviders.GetOrAdd(tenant.Id, (key) => new Lazy<IServiceProvider>(() =>
            {
                //Add all default services
                var container = new ServiceCollection();
                foreach (var service in containerBuilder)
                    container.Add(service);

                //Add tenant specific services
                tenantServiceConfiguration(container, tenant);
                return container.BuildServiceProvider();

            })).Value;
        }
    }

    /// <summary>
    /// Factory wrapper for creating service scopes
    /// </summary>
    /// <param name="serviceProvider"></param>
    internal class MultiTenantServiceScopeFactory<T>(MultiTenantServiceProviderFactory<T> ServiceProviderFactory, IMultiTenantContextAccessor<T> multiTenantContextAccessor) : IMultiTenantServiceScopeFactory where T : ITenantInfo
    {

        /// <summary>
        /// Create scope
        /// </summary>
        /// <returns></returns>
        public IServiceScope CreateScope()
        {
            var tenant = multiTenantContextAccessor.TenantInfo ?? throw new InvalidOperationException("Tenant context is not available");
            return ServiceProviderFactory.GetServiceProviderForTenant(tenant).CreateScope();
        }
    }
}
