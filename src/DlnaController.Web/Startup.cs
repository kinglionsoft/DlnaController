using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using DlnaController.Web.Filters;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace DlnaController.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddOsManager(Environment);

            services.AddUPnPManager();

            services.AddCors(options =>
                            {
                                options.AddPolicy("AllowSpecificOrigin",
                                    builder => builder.WithOrigins(Configuration["cors:origins"].Split(',', StringSplitOptions.RemoveEmptyEntries))
                                                        .AllowAnyHeader()
                                                        .AllowAnyMethod()
                                                        .AllowCredentials());
                            });

            services.AddScoped<CustomExceptionFilterAttribute>();

            services.AddMvc(mvc =>
                {
                    mvc.Filters.Add(typeof(CustomExceptionFilterAttribute));
                    mvc.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Unspecified;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseUPnPManager();

            // Shows UseCors with named policy.
            app.UseCors("AllowSpecificOrigin");

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

                routeBuilder.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}");
            });


            // UPnpTester.Test2(app.ApplicationServices.GetService<ILoggerFactory>());
        }
    }
}
