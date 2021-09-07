using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, List<T>> Group<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            var g = source.GroupBy(keySelector);
            Dictionary<TKey, List<T>> ret = new Dictionary<TKey, List<T>>(g.Count());

            foreach (var kv in g)
            {
                var eles = kv.ToList();
                ret.Add(kv.Key, eles);
            }

            return ret;
        }

        public static List<TResult> CastList<TResult>(this IEnumerable source)
        {
            return source.Cast<TResult>().ToList();
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            Dictionary<TKey, T> dic = new Dictionary<TKey, T>();

            foreach (var item in source)
            {
                TKey key = keySelector(item);
                if (key == null)
                    throw new ArgumentException("source 集合存在 key 为 null 的元素");

                if (dic.ContainsKey(key) == false)
                {
                    dic.Add(key, item);
                }
            }

            return dic.Values;
        }
        public static bool In<T>(this T obj, IEnumerable<T> source)
        {
            return source.Contains(obj);
        }

        public static string Join<T>(this IEnumerable<T> values, string separator = ",")
        {
            if (typeof(T).IsEnum)
            {
                return string.Join(separator, values.Select(a => Convert.ToInt32(a)));
            }

            return string.Join(separator, values);
        }

        /// <summary>
        /// DataTable转实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IEnumerable<T> DataTableToList<T>(this DataTable dt) where T : class, new()
        {
            //判断datatable是否有值
            if (dt.Columns.Count < 1 || dt.Rows.Count < 1)
            { yield break; }
            else
            {
                var propertyInfos = from propertyInfo in typeof(T).GetProperties()
                                    where dt.Columns.Contains(propertyInfo.Name)
                                    select propertyInfo;
                //循环设置属性
                foreach (DataRow dr in dt.Rows)//遍历dt中所有行
                {
                    var result = new T();
                    foreach (var p in propertyInfos)//遍历所有属性
                    {
                        try
                        {
                            if (p.PropertyType.Name == "Int64")
                            {
                                p.SetValue(result, dr[p.Name] == DBNull.Value ? 0 : Convert.ToInt64(dr[p.Name]), null);
                            }
                            else if (p.PropertyType.Name == "Int32")
                            {
                                p.SetValue(result, dr[p.Name] == DBNull.Value ? 0 : Convert.ToInt32(dr[p.Name]), null);
                            }
                            else if (p.PropertyType.Name == "Decimal")
                            {
                                p.SetValue(result, dr[p.Name] == DBNull.Value ? 0 : Convert.ToDecimal(dr[p.Name]), null);
                            }
                            else
                            {
                                p.SetValue(result, dr[p.Name] == DBNull.Value ? null : dr[p.Name], null);
                            }

                        }
                        catch (System.Exception)
                        {

                            throw;
                        }
                    }
                    yield return result;
                }
            }
        }

        /// <summary>
        /// 集合转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
		public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

    }


}