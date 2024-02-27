using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Tests
{
    internal class TestTenant : ITenantInfo
    {
        public required string Id { get; set; }
        public required string Identifier { get; set; }
    }
}
