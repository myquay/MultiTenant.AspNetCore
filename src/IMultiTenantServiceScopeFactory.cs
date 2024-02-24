using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Provides access to the current tenant context
    /// </summary>
    public interface IMultiTenantServiceScopeFactory : IServiceScopeFactory
    { }
}
