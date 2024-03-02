using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Tests
{
    public class OperationIdService
    {
        public readonly Guid Id;

        public OperationIdService()
        {
            Id = Guid.NewGuid();
        }
    }
}
