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
    /// Factory for creating tenant specific service providers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MultiTenantServiceProviderFactory<T>(IServiceCollection containerBuilder, Action<IServiceCollection, T?> tenantServiceConfiguration) where T : ITenantInfo
    {

        //This dictionary keeps track of all of the tenant specific service providers
        private readonly Dictionary<string, IServiceProvider> CompiledProviders = [];
        private readonly object _lock = new();

        public IServiceProvider GetServiceProviderForTenant(T tenant)
        {
            //Only create a new container if needed for performance
            if(CompiledProviders.TryGetValue(tenant.Id, out IServiceProvider? v))
                return v;

            //Add container for tenant
            lock (_lock)
            {
                if (CompiledProviders.TryGetValue(tenant.Id, out IServiceProvider? s))
                    return s;

                //Add all default services
                var container = new ServiceCollection();
                foreach (var service in containerBuilder)
                    container.Add(service);

                //Add tenant specific services
                tenantServiceConfiguration(container, tenant);
                s = container.BuildServiceProvider();

                //Add tenant specific services
                CompiledProviders.Add(tenant.Id, s);

                return s;
            }
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
