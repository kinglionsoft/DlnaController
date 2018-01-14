using DlnaController.OS;
using DlnaController.OS.Internal;
using DlnaController.OS.Internal.Linux;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OsExtensions
    {
        public static IServiceCollection AddOsManager(this IServiceCollection services, IHostingEnvironment env)
        {
            ProcessHelper.WorkingDirectory = env.ContentRootPath;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.AddSingleton<IOsManager, LinuxManager>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.AddSingleton<IOsManager, WindowsManager>();
            }
            return services;
        }
    }
}
