using Microsoft.Extensions.DependencyInjection;

namespace MultiTenant.AspNetCore.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Custom interface to allow registraion of a scoped service that can be resolved within a tenant scope
    /// </summary>
    internal interface IMultiTenantServiceScopeFactory : IServiceScopeFactory
    { }
}
