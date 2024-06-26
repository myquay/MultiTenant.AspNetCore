﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenant.AspNetCore.Tests.Startup
{

    public class TwoTenantStartupNoServicesStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            //Add routing
            services.AddRouting();

            //Add multi-tenant services
            services.AddMultiTenancy<TestTenant>()
                .WithHostResolutionStrategy()
                .WithInMemoryTenantLookupService(new List<TestTenant>
                {
                    new() { Id = "1", Identifier = "tenant1.local" },
                    new() { Id = "2", Identifier = "tenant2.local" }
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/current/tenant-resolution", async context =>
                {
                    var tenantLookUpService = context.RequestServices.GetRequiredService<ITenantResolutionStrategy>();
                    await context.Response.WriteAsync(await tenantLookUpService.GetTenantIdentifierAsync());
                });

                endpoints.MapGet("/current/tenant-accessor", async context =>
                {
                    var tenantAccessor = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor<TestTenant>>();
                    await context.Response.WriteAsync(tenantAccessor.TenantInfo!.Identifier);
                });
            });
        }
    }
}
