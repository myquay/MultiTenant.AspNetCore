using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Nice method to create the tenant builder
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static TenantBuilder AddMultiTenancy(this IServiceCollection services)
        {
            services.AddScoped<ITenantContextService, TenantContextService>();
            return new TenantBuilder(services);
        }
    }
}
