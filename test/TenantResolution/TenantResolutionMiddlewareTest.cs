using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using MultiTenant.AspNetCore.Tests.Startup;
using System.Net;

namespace MultiTenant.AspNetCore.Tests.TenantResolution
{
    public class TenantResolutionMiddlewareTest
    {
        private readonly TestServer _testMultiTenancyServer = new(new WebHostBuilder().UseStartup<TwoTenantStartupNoServicesStartup>());

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

        [Fact]
        public void TenantResolutionInvalidManualMultiTenancyRegistration()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _testInvalidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantInvalidManualMultiTenancyRegistration>());
            });
        }

        [Theory]
        [InlineData("tenant1.local")]
        [InlineData("tenant2.local")]
        public async Task ManualMultiTenantPipelineOrderingBeforeContext(string url)
        {

            var _testValidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantValidManualMultiTenancyRegistration>());
            var context = await _testValidManualMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString(url);
                c.Request.Path = "/before-context";
            });

            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.Empty(await new StreamReader(context.Response.Body).ReadToEndAsync());
        }


        [Theory]
        [InlineData("tenant1.local")]
        [InlineData("tenant2.local")]
        public async Task ManualMultiTenantPipelineOrderingAfterContext(string url)
        {

            var _testValidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantValidManualMultiTenancyRegistration>());

            var context = await _testValidManualMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString(url);
                c.Request.Path = "/after-context";
            });

            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.Equal(url, await new StreamReader(context.Response.Body).ReadToEndAsync());
        }
    }
}
