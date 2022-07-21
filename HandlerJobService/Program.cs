using Microsoft.Extensions.Configuration;
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

namespace HandleJobService
{
    class Program
    {        
        static async Task Main(string[] args)
        {
            await RegisterHelper.RunConfig((services, config) =>
            {
                Web.Quartz.ServiceCollectionExtensions.AddJobFactory(services);
                Web.RegisterConfig.ServiceCollectionExtensions.AddJobService<Program>(services);

                Web.Framework.AutoMapperConfiguration.InitAutoMapper();
                WebCache.ServiceCollectionExtensions.AddCacheServices(services, Globals.Configuration);
                WS.DAL.ServiceCollectionExtensions.AddServices(services, Globals.Configuration);
                Web.MQ.ServiceCollectionExtensions.AddServices(services, Globals.Configuration);
                return services;
            }, args);

        }
    }
}
