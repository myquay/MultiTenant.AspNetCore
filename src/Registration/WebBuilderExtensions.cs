using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiTenant.AspNetCore.Builder;
using MultiTenant.AspNetCore.Infrastructure.DependencyInjection;
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
        public static TenantBuilder<T> AddMultiTenancy<T>(this IServiceCollection Services, Action<MultiTenantOptions<T>>? configure = null) where T : ITenantInfo
        {
            //Register the options
            var options = new MultiTenantOptions<T>();
            configure?.Invoke(options);
            Services.Configure<MultiTenantOptions<T>>(o => configure?.Invoke(o));


            //Provide ambient tenant context
            Services.TryAddSingleton<IMultiTenantContextAccessor<T>, AsyncLocalMultiTenantContextAccessor<T>>();

            //Register middleware to populate the ambient tenant context early in the pipeline
            if (!options.DisableAutomaticPipelineRegistration)
                Services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultiTenantContextAccessorStartupFilter<T>()));

            return new TenantBuilder<T>(Services, options);
        }

        //Register the multitenant request services middleware manually for more control over operational ordering
        public static IApplicationBuilder UseMultiTenancy<T>(this IApplicationBuilder builder)
            where T : ITenantInfo
        {
            //Check if the startup registration is disabled
            var services = builder.ApplicationServices.GetServices<IStartupFilter>()
                .Where(s => s.GetType().IsGenericType);

            if (services.Any(s => s.GetType().GetGenericTypeDefinition() == typeof(MultiTenantContextAccessorStartupFilter<>)))
                throw new InvalidOperationException("UseMultiTenant must only be called if startup registration is disabled, set 'DisableAutomaticPipelineRegistration' to true");

            if (builder.ApplicationServices.GetServices<IMultiTenantServiceScopeFactory>().Any())
            {
                //Register the multitenant request services middleware with the app pipeline
                return builder.UseMiddleware<MultiTenantContextAccessorMiddleware<T>>()
                    .UseMiddleware<MultiTenantRequestServicesMiddleware<T>>();
            }
            else
            {
                return builder.UseMiddleware<MultiTenantContextAccessorMiddleware<T>>();
            }
        }
    }
}
