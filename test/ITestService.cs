using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.Tests
{
    internal interface ITestService
    {
        Guid GetTestValue();
    }

    internal class TestService : ITestService
    {
        private readonly Guid Value = Guid.NewGuid();

        public Guid GetTestValue()
        {
            return Value;
        }
    }
}
