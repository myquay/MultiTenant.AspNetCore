using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Contrib.MultiTenant.Infrastructure;
using Microsoft.AspNetCore.Contrib.MultiTenant.MultiTenantPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Contrib.MultiTenant.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenantPipeline<T>(this IApplicationBuilder builder, Action<T, IApplicationBuilder> configurePipeline)
            where T : class, ITenantInfo
        {
            return builder.UseMiddleware<MultiTenantMiddleware<T>>(builder, configurePipeline);
        }
    }
}
