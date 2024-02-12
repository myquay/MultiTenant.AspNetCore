using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection
{
    public class MultiTenantServiceProvider(IServiceCollection containerBuilder, Action<string, IServiceCollection> tenantServiceConfiguration) : IMultiTenantServiceProvider, ISupportRequiredService
    {
        private readonly IServiceProvider DefaultProvider = containerBuilder.BuildServiceProvider();

        //This dictionary keeps track of all of the tenant scopes that we have created
        private readonly Dictionary<string, IServiceProvider> CompiledProviders = [];

        public object? GetService(Type serviceType)
        {
            var tenantContext = DefaultProvider.GetRequiredService<IMultiTenantContextAccessor>();

            if (tenantContext.TenantId == null)
                return DefaultProvider.GetService(serviceType);

            if (!CompiledProviders.ContainsKey(tenantContext.TenantId))
            {
                //Add all default services
                var container = new ServiceCollection();
                foreach (var service in containerBuilder)
                    container.Add(service);

                //Add tenant specific services
                tenantServiceConfiguration(tenantContext.TenantId, container);

                //Add tenant specific services
                CompiledProviders.Add(tenantContext.TenantId, container.BuildServiceProvider());
            }

            return CompiledProviders[tenantContext.TenantId].GetService(serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetService(serviceType) ?? throw new InvalidOperationException($"Service of type {serviceType} not found");
        }

    }

    public class MultiTenantServiceProviderFactory(Action<string, IServiceCollection> tenantServices) : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services) => services;

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) =>
            new MultiTenantServiceProvider(containerBuilder, tenantServices);
    }
}
