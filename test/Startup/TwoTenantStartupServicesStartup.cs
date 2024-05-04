using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenant.AspNetCore.Tests.Startup
{
    public class TwoTenantStartupServicesStartup
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
                }).WithTenantedServices((s, t) =>
                {
                    s.AddSingleton(new OperationIdService());
                });

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/current/operation-id", async context =>
                {
                    var operationService = context.RequestServices.GetRequiredService<OperationIdService>();
                    await context.Response.WriteAsync($"{operationService.Id}");
                });
            });
        }
    }
}
