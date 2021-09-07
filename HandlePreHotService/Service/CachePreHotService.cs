using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Web.Framework;
using Autofac;
using System.Linq;
using RestSharp;
using System.Collections.Generic;
using WS.BLL.Interface;

namespace HandlePreHotService.Service
{
    //缓存预热服务
    public class CachePreHotService : BackgroundService, IBackDoor
    {
        public IServiceProvider Services { get; set; }
        public ILogger Logger { get; set; }

        private IComponentContext _componentContext;

        public CachePreHotService(IServiceProvider services, IComponentContext componentContext)
        {
            this.Services = services;
            this._componentContext = componentContext;
            this.Logger = services.GetService<ILoggerFactory>().CreateLogger(typeof(CachePreHotService).Name);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("开始预热");
            var result = new SystemResult();

            using var scope = Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IPreHotProductQtyBll>();
            
            try
            {             
                result = await service.CreatePreHeat();
                SaveLog($"处理了Qty的预热", result.Succeeded);
                Console.WriteLine("所有预热完成了");
            }
            catch (Exception ex)
            {
                Console.WriteLine("预热失败");
                Logger.LogError("\r\n 出现异常类型：" + ex.GetType().FullName + "\r\n 异常源：" + ex.Source + "\r\n 异常位置=" + ex.TargetSite + " \r\n 异常信息=" + ex.Message + " \r\n 异常堆栈：" + ex.StackTrace);               
            }           
        }

        private void SaveLog(string msg, bool flag)
        {
            if (flag)
                Logger.LogTrace(msg);
            else
                Logger.LogError(msg);
            Console.WriteLine(msg);
        }

    }
}
