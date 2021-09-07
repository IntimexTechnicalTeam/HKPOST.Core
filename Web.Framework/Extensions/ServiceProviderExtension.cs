using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ServiceProviderExtension
    {
        public static T Resolve<T>(this IServiceProvider services)
        {
            return (T)services.GetService(typeof(T));
        }

        public static ILogger<T> CreateLogger<T>(this IServiceProvider services)
        {
            ILoggerFactory loggerFactory = services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            ILogger<T> logger = loggerFactory.CreateLogger<T>();
            return logger;
        }

        public static ILogger CreateLogger(this IServiceProvider services, Type type)
        {
            ILoggerFactory loggerFactory = services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            ILogger logger = loggerFactory.CreateLogger(type);
            return logger;
        }

        /// <summary>
        /// 从IServiceProvider中获取并解析对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public static T Analyze<T>(this IServiceProvider service)
        {
            return (T)service.GetService(typeof(T));
        }

    }
}
