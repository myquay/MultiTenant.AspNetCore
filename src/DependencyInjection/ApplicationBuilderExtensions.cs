using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenantRequestLocalization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MultiTenantRequestLocalizationMiddleware>();
        }
    }
}
