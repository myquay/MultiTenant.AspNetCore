using Microsoft.Extensions.Options;

namespace MultiTenant.AspNetCore.Infrastructure.Options
{
    internal class MultiTenantOptionsManager<TOptions>(IOptionsFactory<TOptions> factory, IOptionsMonitorCache<TOptions> cache) : IOptionsSnapshot<TOptions> where TOptions : class
    {
        public TOptions Value => Get(Microsoft.Extensions.Options.Options.DefaultName);

        public TOptions Get(string? name)
        {
            name ??= Microsoft.Extensions.Options.Options.DefaultName;
            return cache.GetOrAdd(name, () => factory.Create(name));
        }
    }
}
