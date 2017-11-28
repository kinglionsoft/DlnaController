using Microsoft.AspNetCore.Builder;
using System;
using UPnP.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UPnPMangerExtensions
    {
        /// <summary>
        /// 添加HttpClient池
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddUPnPManager(this IServiceCollection services)
        {
            services.AddSingleton<UpnpManager>();
            return services;
        }

        /// <summary>
        /// 设置租户Id，即停车场Id
        /// </summary>
        /// <param name="app"></param>
        /// <param name="tenentId"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseUPnPManager(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<UpnpManager>().Start();
            return app;
        }
    }
}
