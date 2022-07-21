using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Framework;

namespace WebCache
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注入CSRedis
        /// </summary>
        /// <param name="services"></param>s
        /// <param name="t"></param>
        /// <returns></returns>
        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration config)
        {
            string conn = config["Redis:conn"];
            var csredis = new CSRedisClient(conn);
            RedisHelper.Initialization(csredis);
            services.AddSingleton(csredis);           
            return services;
        }
    }
}
