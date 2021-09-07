using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Web.Framework
{
    public static class AutoMapperConfiguration
    {
        public static void InitAutoMapper()
        {
            var assemblys = RuntimeHelper.Discovery().ToList();
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                //在此添加代码
                assemblys.ForEach(a =>
                {
                    try
                    {
                        var types = a.GetTypes();
                        var profileList = types.Where(t => t.GetTypeInfo().IsClass && typeof(ICreateMapper).IsAssignableFrom(t)).ToList();
                        if (profileList != null && profileList.Any())
                        {
                            profileList.ForEach(t =>
                            {
                                cfg.AddProfile(Activator.CreateInstance(t) as Profile);
                            });
                        }
                    }
                    catch
                    {
                    }
                });

            });
            Mapper = MapperConfiguration.CreateMapper();
        }
        public static IMapper Mapper { get; private set; }
        public static MapperConfiguration MapperConfiguration { get; private set; }
    }

}
