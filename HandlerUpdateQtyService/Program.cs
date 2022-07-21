using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.AutoFac;
using Web.Framework;
using Web.RegisterConfig;

/// <summary>
/// 缓存预热服务
/// </summary>
namespace HandleUpdateQtyService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await RegisterHelper.RunConfig((services, config) =>
            {
                //依赖注入
                Web.Framework.AutoMapperConfiguration.InitAutoMapper();
                Web.RegisterConfig.ServiceCollectionExtensions.AddServices<Program>(services, Globals.Configuration);
                WebCache.ServiceCollectionExtensions.AddCacheServices(services, Globals.Configuration);
                WS.DAL.ServiceCollectionExtensions.AddServices(services, Globals.Configuration);
                return services;
            }, args);

        }
    }
}
