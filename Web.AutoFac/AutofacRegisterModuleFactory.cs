using Autofac;
using Autofac.Extras.DynamicProxy;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Web.Framework;
using WS.DAL;

namespace Web.AutoFac
{
    /// <summary>
    /// autofac注入工厂类
    /// </summary>
    public class AutofacRegisterModuleFactory : Autofac.Module
    {
        //重写Autofac管道Load方法，在这里注册注入
        protected override void Load(ContainerBuilder builder)
        {
            //注入上下文
            builder.RegisterType(typeof(MallDbContext)).As(typeof(MallDbContext)).InstancePerLifetimeScope().AsImplementedInterfaces();

            var assemblys = RuntimeHelper.Discovery();

            //////只要继承IDependency就自动注册
            builder.RegisterAssemblyTypes(assemblys)
                      .Where(t => !t.GetTypeInfo().IsAbstract && typeof(IDependency).IsAssignableFrom(t))
                      .EnableInterfaceInterceptors()//AOP拦截
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope();////自动注册    
        }
    }
}
