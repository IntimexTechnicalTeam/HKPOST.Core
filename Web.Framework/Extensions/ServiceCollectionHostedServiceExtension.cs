using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionHostedServiceExtension
    {
        public static IServiceCollection AddHostedService<THostedService>(this IServiceCollection services, string consumers) where THostedService : class, IHostedService
        {
            int consumersT = 0;
            int.TryParse(consumers, out consumersT);
            return AddHostedService<THostedService>(services, consumersT);
        }
        public static IServiceCollection AddHostedService<THostedService>(this IServiceCollection services, int consumers) where THostedService : class, IHostedService
        {
            if (consumers == 0)
            {
                consumers = 1;
            }

            for (int i = 0; i < consumers; i++)
            {
                services.AddHostedService<THostedService>();
            }

            return services;
        }
    }
}
