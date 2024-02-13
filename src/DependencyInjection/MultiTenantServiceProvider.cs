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
    internal class MultiTenantServiceProvider<T>(IServiceCollection containerBuilder, Action<T, IServiceCollection> tenantServiceConfiguration) : IMultiTenantServiceProvider, ISupportRequiredService where T : ITenantInfo
    {
        private readonly IServiceProvider DefaultProvider = containerBuilder.BuildServiceProvider();

        //This dictionary keeps track of all of the tenant scopes that we have created
        private readonly Dictionary<string, IServiceProvider> CompiledProviders = [];

        public object? GetService(Type serviceType)
        {
            var tenantContext = DefaultProvider.GetRequiredService<IMultiTenantContextAccessor<T>>();

            if (tenantContext.TenantInfo == null)
                return DefaultProvider.GetService(serviceType);

            if (!CompiledProviders.ContainsKey(tenantContext.TenantInfo.Id))
            {
                //Add all default services
                var container = new ServiceCollection();
                foreach (var service in containerBuilder)
                    container.Add(service);

                //Add tenant specific services
                tenantServiceConfiguration(tenantContext.TenantInfo, container);

                //Add tenant specific services
                CompiledProviders.Add(tenantContext.TenantInfo.Id, container.BuildServiceProvider());
            }

            return CompiledProviders[tenantContext.TenantInfo.Id].GetService(serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetService(serviceType) ?? throw new InvalidOperationException($"Service of type {serviceType} not found");
        }

    }

    internal class MultiTenantServiceProviderFactory<T>(Action<T, IServiceCollection> tenantServices) : IServiceProviderFactory<IServiceCollection> where T : ITenantInfo
    {
        public IServiceCollection CreateBuilder(IServiceCollection services) => services;

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) =>
            new MultiTenantServiceProvider<T>(containerBuilder, tenantServices);
    }
}
