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

namespace HandleTestJobService
{
    class Program
    {
        public static string ServiceName = Process.GetCurrentProcess().ProcessName;

        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = new HostBuilder()
                .ConfigureLogging(factory =>
                {
                    factory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                    NLog.LogManager.LoadConfiguration("Config/nlog.config");
                })
              .ConfigureHostConfiguration(config =>
              {
                  if (args != null)
                  {
                      config.AddCommandLine(args);
                  }
                  config.SetBasePath(Directory.GetCurrentDirectory());
                  config.AddEnvironmentVariables("ASPNETCORE_");
              })
              .ConfigureAppConfiguration((hostContext, config) =>
              {
                  config.AddJsonFile("Config/appsettings.json", true, true);
                  config.AddEnvironmentVariables();
                  
                  if (args != null)
                  {
                      config.AddCommandLine(args);
                  }
                  Globals.Configuration = config.Build();           //生成全局IConfigration
                  //Snowflake.machineId = ObjConvert.TryInt64(Globals.Configuration["WorkerID"]);
              })
              .UseServiceProviderFactory(new CustomAutofacServiceProviderFactory())
              .ConfigureServices(services =>
              {
                  Web.Quartz.ServiceCollectionExtensions.AddJobFactory(services);           
                  ServiceCollectionExtensions.AddJobService(services);
              }
            );

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }
            if (isService)
            {
                Console.WriteLine($"{ServiceName}开启...");
                await builder.RunConsoleAsync();
                Console.WriteLine($"{ServiceName}停止");
            }
            else
            {
                var host = builder.Build();
                using (host)
                {
                    Console.WriteLine($"{ServiceName}开启...");
                    await host.StartAsync();
                    Console.ReadKey(true);
                    Console.WriteLine($"{ServiceName}停止");
                    await host.WaitForShutdownAsync();
                }
            }
        }
    }
}
