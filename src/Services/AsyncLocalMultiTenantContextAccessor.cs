using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Services
{
    internal class AsyncLocalMultiTenantContextAccessor<W> : IMultiTenantContextAccessor<W> where W : ITenantInfo
    {
        private static readonly AsyncLocal<W?> AsyncLocalContext = new();

        public W? TenantInfo { get => AsyncLocalContext.Value; set => AsyncLocalContext.Value = value; }
    }
}
