using Microsoft.AspNetCore.Builder;
using MultiTenant.AspNetCore.Infrastructure.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenantPipeline<T>(this IApplicationBuilder builder, Action<T, IApplicationBuilder> configurePipeline)
            where T : ITenantInfo
        {
            return builder.UseMiddleware<MultiTenantMiddleware<T>>(builder, configurePipeline);
        }
    }
}
