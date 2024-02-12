using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Services
{
    internal class AsyncLocalMultiTenantContextAccessor : IMultiTenantContextAccessor
    {
        private static readonly AsyncLocal<string?> AsyncLocalContext = new();

        public string? TenantId { get => AsyncLocalContext.Value; set => AsyncLocalContext.Value = value; }
    }
}
