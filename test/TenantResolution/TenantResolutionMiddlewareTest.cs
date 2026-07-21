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
        private readonly TestServer _returnNotFoundServer = new(new WebHostBuilder().UseStartup<TwoTenantReturnNotFoundStartup>());

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
        public async Task ReturnNotFoundBehaviorInvalidTenantReturns404()
        {
            var context = await _returnNotFoundServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString("public-web.azurewebsites.net");
                c.Request.Path = "/current/tenant-accessor";
            });

            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
            Assert.Empty(await new StreamReader(context.Response.Body).ReadToEndAsync());
        }

        [Fact]
        public async Task ReturnNotFoundBehaviorValidTenantContinuesPipeline()
        {
            var context = await _returnNotFoundServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString("tenant1.local");
                c.Request.Path = "/current/tenant-accessor";
            });

            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.Equal("tenant1.local", await new StreamReader(context.Response.Body).ReadToEndAsync());
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
        public async Task ManualMultiTenantPipelineOrderingBeforeContextNoServices(string url)
        {

            var testValidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantValidManualMultiTenancyRegistrationNoServices>());
            var context = await testValidManualMultiTenancyServer.SendAsync(c =>
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
        public async Task ManualMultiTenantPipelineOrderingAfterContextNoServices(string url)
        {

            var testValidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantValidManualMultiTenancyRegistrationNoServices>());

            var context = await testValidManualMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString(url);
                c.Request.Path = "/after-context";
            });

            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.Equal(url, await new StreamReader(context.Response.Body).ReadToEndAsync());
        }

        [Theory]
        [InlineData("tenant1.local")]
        [InlineData("tenant2.local")]
        public async Task ManualMultiTenantPipelineOrderingBeforeContextWithServices(string url)
        {

            var testValidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantValidManualMultiTenancyRegistrationWithServices>());
            var context = await testValidManualMultiTenancyServer.SendAsync(c =>
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
        public async Task ManualMultiTenantPipelineOrderingAfterContextWithServices(string url)
        {

            var testValidManualMultiTenancyServer = new TestServer(new WebHostBuilder().UseStartup<TwoTenantValidManualMultiTenancyRegistrationWithServices>());

            var context = await testValidManualMultiTenancyServer.SendAsync(c =>
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
