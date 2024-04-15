using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiTenant.AspNetCore.Builder;
using MultiTenant.AspNetCore.Infrastructure.Middleware;

namespace MultiTenant.AspNetCore
{
    /// <summary>
    /// Nice method to create the tenant builder
    /// </summary>
    public static class WebBuilderExtensions
    {
        /// <summary>
        /// Add the services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static TenantBuilder<T> AddMultiTenancy<T>(this IServiceCollection Services) where T : ITenantInfo
        {
            //Provide ambient tenant context
            Services.TryAddSingleton<IMultiTenantContextAccessor<T>, AsyncLocalMultiTenantContextAccessor<T>>();

            //Register middleware to populate the ambient tenant context early in the pipeline
            Services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultiTenantContextAccessorStartupFilter<T>()));

            return new TenantBuilder<T>(Services);
        }
    }
}
