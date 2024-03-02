using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Tests.TenantResolution
{
    public class TenantResolutionMiddlewareTest
    {
        private readonly TestServer _testMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantStartupNoServicesStartup>());

        [Theory]
        [InlineData("tenant1.local")]
        [InlineData("tenant2.local")]
        public async Task TenantResolutionServiceValidTenant(string url)
        {

            var context = await _testMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString(url);
                c.Request.Path = "/current/tenant-resolution";
            });

            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.Equal(url, await new StreamReader(context.Response.Body).ReadToEndAsync());
        }

        [Theory]
        [InlineData("tenant1.local")]
        [InlineData("tenant2.local")]
        public async Task TenantAccessorValidTenant(string url)
        {

            var context = await _testMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString(url);
                c.Request.Path = "/current/tenant-accessor";
            });

            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.Equal(url, await new StreamReader(context.Response.Body).ReadToEndAsync());
        }

        [Theory]
        [InlineData("invalid-tenant.local")]
        public async Task TenantResolutionServiceInvalidTenant(string url)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var context = await _testMultiTenancyServer.SendAsync(c =>
                {
                    c.Request.Method = HttpMethods.Get;
                    c.Request.Host = new HostString(url);
                    c.Request.Path = "/current/tenant-resolution";
                });
            });
        }
    }

    public class  TwoTenantStartupNoServicesStartup
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
