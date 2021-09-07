using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;
using Web.Framework;

namespace Web.AutoFac
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册单例服务,必须继承IBackDoor
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            
            ////找到当前的程序集
            var assemblys = RuntimeHelper.Discovery().ToList().Where(o => o.GetName().Name.Equals(MethodBase.GetCurrentMethod().DeclaringType.Namespace)).ToList();
            assemblys.FirstOrDefault().DefinedTypes.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IBackDoor).IsAssignableFrom(t)).ToList().ForEach(t =>
            {
                services.AddSingleton(typeof(IHostedService), t);
            });

            return services;
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <param name="services"></param>s
        /// <param name="t"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, Type t)
        {
            services.AddSingleton(t);
            return services;
        }
    }
}
