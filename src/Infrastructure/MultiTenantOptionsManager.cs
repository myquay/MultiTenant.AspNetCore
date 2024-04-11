using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure
{
    internal class MultiTenantOptionsManager<TOptions>(IOptionsFactory<TOptions> factory, IOptionsMonitorCache<TOptions> cache) : IOptionsSnapshot<TOptions> where TOptions : class
    {
        public TOptions Value => Get(Options.DefaultName);

        public TOptions Get(string? name)
        {
            name ??= Microsoft.Extensions.Options.Options.DefaultName;
            return cache.GetOrAdd(name, () => factory.Create(name));
        }
    }
}
