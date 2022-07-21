using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;

namespace Web.RegisterConfig
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册Job和Service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJobService<T>(this IServiceCollection services)
        {
            ////找到当前的程序集 MethodBase.GetCurrentMethod().DeclaringType.Namespace
            var assemblys = RuntimeHelper.Discovery().ToList().Where(o => o.GetName().Name.Equals(typeof(T).Namespace)).ToList();

            assemblys.FirstOrDefault().DefinedTypes.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IJob).IsAssignableFrom(t)).ToList().ForEach(t =>
            {
                services.AddTransient(t.AsType());
            });

            assemblys.FirstOrDefault().DefinedTypes.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IBackDoor).IsAssignableFrom(t)).ToList().ForEach(t =>
            {
                services.AddSingleton(typeof(IHostedService), t);
            });

            return services;
        }

        public static IServiceCollection AddServices<T>(this IServiceCollection services, IConfiguration config)
        {

            ////找到当前的程序集  MethodBase.GetCurrentMethod().DeclaringType.Namespace
            var assemblys = RuntimeHelper.Discovery().ToList().Where(o => o.GetName().Name.Equals(typeof(T).Namespace)).ToList();
            assemblys.FirstOrDefault().DefinedTypes.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IBackDoor).IsAssignableFrom(t)).ToList().ForEach(t =>
            {
                services.AddSingleton(typeof(IHostedService), t);
            });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, Type t)
        {
            services.AddSingleton(t);
            return services;
        }
    }
}
