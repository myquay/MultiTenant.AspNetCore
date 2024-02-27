using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using Microsoft.AspNetCore.Contrib.MultiTenant.Middleware;
using Microsoft.AspNetCore.Contrib.MultiTenant.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection
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
            Services.AddScoped<IMultiTenantContextAccessor<T>, AsyncLocalMultiTenantContextAccessor<T>>();

            //Register middleware to populate the ambient tenant context early in the pipeline
            Services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultiTenantContextAccessorStartupFilter<T>()));

            return new TenantBuilder<T>(Services);
        }
    }
}
