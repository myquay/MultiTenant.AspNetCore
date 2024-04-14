using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant
{
    /// <summary>
    /// Custom interface to allow registraion of a scoped service that can be resolved within a tenant scope
    /// </summary>
    public interface IMultiTenantServiceScopeFactory : IServiceScopeFactory
    { }
}
