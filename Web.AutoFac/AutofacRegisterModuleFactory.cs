using Autofac;
using Autofac.Extras.DynamicProxy;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Web.Framework;


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
            var assemblys = RuntimeHelper.Discovery();

            //////只要继承IDependency就自动注册
            builder.RegisterAssemblyTypes(assemblys)
                      .Where(t => !t.GetTypeInfo().IsAbstract && typeof(IDependency).IsAssignableFrom(t))
                      .EnableInterfaceInterceptors()//AOP拦截
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope();////自动注册    

            // builder.RegisterAssemblyTypes(assemblys)
            //.Where(t => !t.GetTypeInfo().IsAbstract && typeof(IDal).IsAssignableFrom(t))
            //.InstancePerLifetimeScope();////自动注册    

            //注入SqlSugarDbContext,泛型注入
            //builder.RegisterGeneric(typeof(SugarDbContext<>)).As(typeof(ISugarDbContext<>)).InstancePerLifetimeScope().AsImplementedInterfaces();
            //builder.RegisterType(typeof(SugarDbContext)).As(typeof(ISugarDbContext)).InstancePerLifetimeScope().AsImplementedInterfaces();

            //针对一个接口，多个实现进行注入
            //builder.RegisterType<PreHeatPromotionViewService>().Named<IBasePreHeatService>(typeof(PreHeatPromotionViewService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<PreHeatPromotionProductService>().Named<IBasePreHeatService>(typeof(PreHeatPromotionProductService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<PreHeatPromotionBannerService>().Named<IBasePreHeatService>(typeof(PreHeatPromotionBannerService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<PreHeatPromotionMerchantService>().Named<IBasePreHeatService>(typeof(PreHeatPromotionMerchantService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();

            //builder.RegisterType<PreHeatProductService>().Named<IBasePreHeatService>(typeof(PreHeatProductService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<PreHeatProductImageService>().Named<IBasePreHeatService>(typeof(PreHeatProductImageService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<PreHeatMerchantService>().Named<IBasePreHeatService>(typeof(PreHeatMerchantService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<PreHeatMchFeeChargeService>().Named<IBasePreHeatService>(typeof(PreHeatMchFeeChargeService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();

            //builder.RegisterType<PreFavoriteService>().Named<IBasePreHeatService>(typeof(PreFavoriteService).Name).EnableInterfaceInterceptors().AsImplementedInterfaces().InstancePerLifetimeScope();

        }
    }
}
