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
        public static TenantBuilder AddMultiTenancy(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ITenantContextService, TenantContextService>();
            builder.Services.AddScoped<IMultiTenantContextAccessor, AsyncLocalMultiTenantContextAccessor>();

            return new TenantBuilder(builder);
        }
    }
}
