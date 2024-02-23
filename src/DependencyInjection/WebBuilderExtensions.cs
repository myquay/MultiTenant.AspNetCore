using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using Microsoft.AspNetCore.Contrib.MultiTenant.Services;
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
        public static TenantBuilder<T> AddMultiTenancy<T>(this WebApplicationBuilder builder) where T : ITenantInfo
        {
            builder.Services.AddTransient<TenantMiddleware<T>>();
            builder.Services.AddScoped<IMultiTenantContextAccessor<T>, AsyncLocalMultiTenantContextAccessor<T>>();

            return new TenantBuilder<T>(builder);
        }
    }
}
