using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Service provider for tenant aware services
    /// </summary>
    public interface IMultiTenantServiceProvider : IServiceProvider
    { }
}
