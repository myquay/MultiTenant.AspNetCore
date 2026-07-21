using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenant.AspNetCore.Tests.Startup;

public class TwoTenantReturnNotFoundStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();

        services.AddMultiTenancy<TestTenant>(options =>
        {
            options.MissingTenantBehavior = MissingTenantBehavior.ReturnNotFound;
        })
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
            endpoints.MapGet("/current/tenant-accessor", async context =>
            {
                var tenantAccessor = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor<TestTenant>>();
                await context.Response.WriteAsync(tenantAccessor.TenantInfo!.Identifier);
            });
        });
    }
}
