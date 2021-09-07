using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.AutoFac
{
    /// <summary>
    /// 控制台程序的注入工厂类,只要控制台注册CustomAutofacServiceProviderFactory，将会完成CreateServiceProvider
    /// </summary>
    public class CustomAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        AutofacServiceProviderFactory _factory = new AutofacServiceProviderFactory();
        public CustomAutofacServiceProviderFactory()
        {

        }
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = _factory.CreateBuilder(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AutofacRegisterModuleFactory>();
            IContainer container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);
            return serviceProvider;
        }
    }
}

