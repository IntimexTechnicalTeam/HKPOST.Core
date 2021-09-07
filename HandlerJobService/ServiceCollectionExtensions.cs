using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Web.Framework;

namespace HandleTestJobService
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 使用反射自动注册Job和Job服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobService(this IServiceCollection services)
        {
            ////找到当前的程序集
            var assemblys = RuntimeHelper.Discovery().ToList().Where(o => o.GetName().Name.Equals(MethodBase.GetCurrentMethod().DeclaringType.Namespace)).ToList();

            assemblys.FirstOrDefault().DefinedTypes.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IJob).IsAssignableFrom(t)).ToList().ForEach(t =>
            {
                services.AddTransient(t.AsType());
            });

            assemblys.FirstOrDefault().DefinedTypes.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IBackDoor).IsAssignableFrom(t)).ToList().ForEach(t =>
            {     
                services.AddSingleton(typeof(IHostedService), t);
            });

            ////已更改为上述代码，可自动注册
            //services.AddTransient<TestJob>();
            //services.AddHostedService<TestJobService>();

            return services;
        }
    }
}
