using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.MQ;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Web.MQ
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注入RabbitMQ
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            Action<RabbitMQOptions> mqOptionsConfigure = op =>
            {
                op.HostName = config["RabbitMQ:HostName"];
                op.UserName = config["RabbitMQ:UserName"];
                op.Password = config["RabbitMQ:Password"];
                op.VirtualHost = "/";

                if (!int.TryParse(config["RabbitMQ:Port"], out var port)) port = 5672;
                op.Port = port;
            };

            return AddServices(services, mqOptionsConfigure);
        }
        public static IServiceCollection AddServices(this IServiceCollection services, Action<RabbitMQOptions> mqOptionsConfigure)
        {
            services.Configure(mqOptionsConfigure);
            services.AddSingleton<IConnectionChannelPool, ConnectionChannelPool>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();

            return services;
        }
    }
}
