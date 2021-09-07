using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Web.Framework
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string ObjectToJson(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T JsonToObject<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        ///// <summary>
        ///// 解析JSON数组生成对象实体集合
        ///// </summary>
        ///// <typeparam name="T">对象类型</typeparam>
        ///// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        ///// <returns>对象实体集合</returns>
        //public static List<T> DeserializeJsonToList<T>(string json) where T : class
        //{
        //    JsonSerializer serializer = new JsonSerializer();
        //    StringReader sr = new StringReader(json);
        //    object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
        //    List<T> list = o as List<T>;
        //    return list;
        //}

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }

        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <param name="obj">待转化的对象</param>
        /// <param name="isIgnoreNull">是否忽略NULL 这里我不需要转化NULL的值，正常使用可以不穿参数 默认全转换</param>
        /// <returns></returns>
        public static Dictionary<string, object> ObjectToMap(object obj, bool isIgnoreNull = false)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理 
                    if (m.Invoke(obj, new object[] { }) != null || !isIgnoreNull)
                    {
                        map.Add(p.Name, m.Invoke(obj, new object[] { })); // 向字典添加元素
                    }
                }
            }
            return map;
        }

        /// <summary>
        /// 字典转实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T ConvertDic<T>(Dictionary<string, object> dic)
        {
            T model = Activator.CreateInstance<T>();
            PropertyInfo[] modelPro = model.GetType().GetProperties();
            if (modelPro.Length > 0 && dic.Count() > 0)
            {
                for (int i = 0; i < modelPro.Length; i++)
                {
                    if (dic.ContainsKey(modelPro[i].Name))
                    {
                        modelPro[i].SetValue(model, dic[modelPro[i].Name], null);
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 对象转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asObject"></param>
        /// <returns></returns>
        public static T ConvertObject<T>(object asObject) where T : new()
        {
            //创建实体对象实例
            var t = Activator.CreateInstance<T>();
            if (asObject != null)
            {
                Type type = asObject.GetType();
                //遍历实体对象属性
                foreach (var info in typeof(T).GetProperties())
                {
                    object obj = null;
                    //取得object对象中此属性的值
                    var val = type.GetProperty(info.Name)?.GetValue(asObject);
                    if (val != null)
                    {
                        //非泛型
                        if (!info.PropertyType.IsGenericType)
                            obj = Convert.ChangeType(val, info.PropertyType);
                        else//泛型Nullable<>
                        {
                            Type genericTypeDefinition = info.PropertyType.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Nullable<>))
                            {
                                obj = Convert.ChangeType(val, Nullable.GetUnderlyingType(info.PropertyType));
                            }
                            else
                            {
                                obj = Convert.ChangeType(val, info.PropertyType);
                            }
                        }
                        info.SetValue(t, obj, null);
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 判断对象是json数组还是json对象，
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static JSON_TYPE getJSONType(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return JSON_TYPE.JSON_TYPE_ERROR;
            }


            char[] strChar = str.Substring(0, 1).ToCharArray();
            char firstChar = strChar[0];


            if (firstChar == '{')
            {
                return JSON_TYPE.JSON_TYPE_OBJECT;
            }
            else if (firstChar == '[')
            {
                return JSON_TYPE.JSON_TYPE_ARRAY;
            }
            else
            {
                return JSON_TYPE.JSON_TYPE_ERROR;
            }
        }

        /// <summary>
        /// json类型
        /// </summary>
        public enum JSON_TYPE
        {
            /**JSONObject*/
            JSON_TYPE_OBJECT,
            /**JSONArray*/
            JSON_TYPE_ARRAY,
            /**不是JSON格式的字符串*/
            JSON_TYPE_ERROR
        }
        /// <summary>
        /// List转datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        //public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        //{
        //    var props = typeof(T).GetProperties();
        //    var dt = new DataTable();
        //    dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
        //    if (collection.Count() > 0)
        //    {
        //        for (int i = 0; i < collection.Count(); i++)
        //        {
        //            ArrayList tempList = new ArrayList();
        //            foreach (PropertyInfo pi in props)
        //            {
        //                object obj = pi.GetValue(collection.ElementAt(i), null);
        //                tempList.Add(obj);
        //            }
        //            object[] array = tempList.ToArray();
        //            dt.LoadDataRow(array, true);
        //        }
        //    }

        //    return dt;

        //}

        /// <summary>

        /// Convert a List{T} to a DataTable.

        /// </summary>

       public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];



                for (int i = 0; i < props.Length; i++)

                {

                    values[i] = props[i].GetValue(item, null);

                }



                tb.Rows.Add(values);

            }



            return tb;

        }

        /// <summary>

        /// Return underlying type if type is Nullable otherwise return the type

        /// </summary>

        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {

                if (!t.IsValueType)

                {

                    return t;

                }

                else

                {

                    return Nullable.GetUnderlyingType(t);

                }

            }

            else

            {

                return t;

            }

        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>

        public static bool IsNullable(Type t)

        {

            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));

        }
    }
}
