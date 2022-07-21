using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Extensions.Logging;
using Web.AutoFac;
using Web.Framework;

namespace Web.RegisterConfig
{
    public static class RegisterHelper
    {
        /// <summary>
        /// 控制台程序专用的启动配置
        /// </summary>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task RunConfig(Func<IServiceCollection, IConfiguration, IServiceCollection> func, string[] args)
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
                  Globals.Configuration = config.Build();
              })
              .UseServiceProviderFactory(new CustomAutofacServiceProviderFactory())
              .ConfigureServices(services =>
              {
                  func.Invoke(services, Globals.Configuration);
              });

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            string ServiceName = Process.GetCurrentProcess().ProcessName;

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
