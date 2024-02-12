using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Strategies
{
    /// <summary>
    /// Resolve the host to a tenant identifier
    /// </summary>
    public class HostResolutionStrategy(IHttpContextAccessor httpContextAccessor) : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Get the tenant identifier
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> GetTenantIdentifierAsync()
        {
            if (_httpContextAccessor.HttpContext == null)
                throw new InvalidOperationException("HttpContext is not available");

            return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
        }
    }
}
