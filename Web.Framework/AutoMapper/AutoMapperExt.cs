using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Framework
{
    public static class AutoMapperExt
    {
        public static T MapTo<T>(this object obj)
        {
            return null == obj ? default(T) : AutoMapperConfiguration.Mapper.Map<T>(obj);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        public static TDestination MapToList<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        //注：这个没有测试过是否可用。上面那个MapToList是可用的
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            return AutoMapperConfiguration.Mapper.Map<List<TDestination>>(source);
        }
    }

}
