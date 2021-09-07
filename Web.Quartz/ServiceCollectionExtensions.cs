using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Web.Quartz
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {     
        /// <summary>
        /// 注册JobFactory
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobFactory(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, JobFactorys>(); //Job工厂
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//注册ISchedulerFactory的实例
            services.AddSingleton(p =>
            {
                var sf = new StdSchedulerFactory();
                var scheduler = sf.GetScheduler().Result;
                scheduler.JobFactory = p.GetService<IJobFactory>();
                return scheduler;
            });//注册ISchedulerFactory的实例。

            return services;
        }
    }
}
