using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using MultiTenant.AspNetCore.Tests.Startup;

namespace MultiTenant.AspNetCore.Tests.TenantResolution
{
    public class TenantServiceContainerTest
    {
        private readonly TestServer _testMultiTenancyServer = new(new WebHostBuilder().UseStartup<TwoTenantStartupServicesStartup>());

        [Fact]
        public async Task DifferentOperationServiceInstances()
        {
            var context1Request1 = await _testMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString("tenant1.local");
                c.Request.Path = "/current/operation-id";
            });

            var context2Request1 = await _testMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString("tenant2.local");
                c.Request.Path = "/current/operation-id";
            });

            var context1Request2 = await _testMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString("tenant1.local");
                c.Request.Path = "/current/operation-id";
            });

            var context2Request2 = await _testMultiTenancyServer.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Get;
                c.Request.Host = new HostString("tenant2.local");
                c.Request.Path = "/current/operation-id";
            });

            var context1 = (await Task.WhenAll(
                                new StreamReader(context1Request1.Response.Body).ReadToEndAsync(),
                                new StreamReader(context1Request2.Response.Body).ReadToEndAsync())).ToList();

            var context2 = (await Task.WhenAll(
                                new StreamReader(context2Request1.Response.Body).ReadToEndAsync(),
                                new StreamReader(context2Request2.Response.Body).ReadToEndAsync())).ToList();

            Assert.Equal(context1[0], context1[1]);
            Assert.Equal(context2[0], context2[1]);
            Assert.NotEqual(context1[0], context2[0]);

        }
    }
}
