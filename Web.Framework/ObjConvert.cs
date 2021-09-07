
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Web.Framework
{
    /// <summary>
    /// 对象转换
    /// </summary>
    public class ObjConvert
    {
        /// <summary>
        /// 转为INT32
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static int TryInt(object obj, int defInt)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defInt;
            }
            string value = obj.ToString();
            if (value.Contains("."))
            {
                return TryInt(TryDecimal(value));
            }
            else
            {
                int outInt = 0;
                if (int.TryParse(obj.ToString(), out outInt))
                {
                    return outInt;
                }
                else
                {
                    return defInt;
                }
            }

        }

        /// <summary>
        /// 转为INT32
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static int TryInt(object obj)
        {
            return TryInt(obj, 0);
        }

        /// <summary>
        /// 转为INT32
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static int TryInt(decimal obj)
        {
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 转为INT32
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static int TryInt(double obj)
        {
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 转为INT16
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static Int16 TryInt16(object obj, Int16 defInt)
        {
            if (obj == null || obj == DBNull.Value)
                return defInt;

            Int16 outInt = 0;
            if (Int16.TryParse(obj.ToString(), out outInt))
            {
                return outInt;
            }
            else
            {
                return defInt;
            }
        }
        /// <summary>
        /// 转为INT16
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static Int16 TryInt16(object obj)
        {
            return TryInt16(obj, 0);
        }

        /// <summary>
        /// 转为short
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="defShort">默认值</param>
        /// <returns></returns>
        public static short TryShort(object obj, short defShort)
        {
            if (obj == null || obj == DBNull.Value)
                return defShort;

            short outShort = 0;
            if (short.TryParse(obj.ToString(), out outShort))
            {
                return outShort;
            }
            else
            {
                return defShort;
            }
        }

        /// <summary>
        /// 转为short
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static short TryShort(object obj)
        {
            return TryShort(obj, 0);
        }

        /// <summary>
        /// 转为decimal
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="defInt">默认值</param>
        /// <returns></returns>
        public static decimal TryDecimal(object obj, decimal defInt)
        {
            if (obj == null || obj == DBNull.Value)
                return defInt;

            decimal outInt = 0;
            if (decimal.TryParse(obj.ToString(), out outInt))
            {
                return outInt;
            }
            else
            {
                return defInt;
            }
        }

        /// <summary>
        /// 转为decimal
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static decimal TryDecimal(object obj)
        {
            return TryDecimal(obj, 0M);
        }

        /// <summary>
        /// 转为float
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="defFloat">默认值</param>
        /// <returns></returns>
        public static float TryFloat(object obj, float defFloat)
        {
            if (obj == null || obj == DBNull.Value)
                return defFloat;

            float outFloat = 0;
            if (float.TryParse(obj.ToString(), out outFloat))
            {
                return outFloat;
            }
            else
            {
                return defFloat;
            }
        }

        /// <summary>
        /// 转为float
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static float TryFloat(object obj)
        {
            return TryFloat(obj, 0);
        }

        /// <summary>
        /// 转为INT64
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static Int64 TryInt64(object obj, Int64 defInt)
        {
            if (obj == null || obj == DBNull.Value)
                return defInt;

            Int64 outInt = 0;

            try
            {
                outInt = Convert.ToInt64(obj);
            }
            catch
            {
                outInt = defInt;
            }
            return outInt;
            //if (Int64.TryParse(obj.ToString(), out outInt))
            //{
            //    return outInt;
            //}
            //else
            //{
            //    return defInt;
            //}
        }

        /// <summary>
        /// 转为INT64
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static Int64 TryInt64(object obj)
        {
            return TryInt64(obj, 0);
        }

        /// <summary>
        /// 转为double
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="defDouble">默认值</param>
        /// <returns></returns>
        public static double TryDouble(object obj, double defDouble)
        {
            if (obj == null || obj == DBNull.Value)
                return defDouble;

            double outDouble = 0;
            if (double.TryParse(obj.ToString(), out outDouble))
            {
                return outDouble;
            }
            else
            {
                return defDouble;
            }
        }

        /// <summary>
        /// 转为double
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static double TryDouble(object obj)
        {
            return TryDouble(obj, 0);
        }


        /// <summary>
        /// 转为string
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static string TryStr(object obj, string defInt)
        {
            if (obj == null || obj == DBNull.Value)
                return defInt;

            return obj.ToString().Trim();
        }

        /// <summary>
        /// 转为string
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static string TryStr(object obj)
        {
            return TryStr(obj, string.Empty);
        }

        /// <summary>
        /// 转为DateTime
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static DateTime TryDateTime(object obj, DateTime defInt)
        {
            if (obj == null || obj == DBNull.Value)
                return defInt;

            DateTime outInt = defInt;
            if (DateTime.TryParse(obj.ToString(), out outInt))
            {
                return outInt;
            }
            else
            {
                return defInt;
            }
        }

        /// <summary>
        /// 转为DateTime
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static DateTime TryDateTime(object obj)
        {
            return TryDateTime(obj, System.DateTime.Now);
        }

        /// <summary>
        /// 转为DateTime
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <returns></returns>
        public static DateTime? TryDateTime2(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;

            DateTime outInt = System.DateTime.Now;
            if (DateTime.TryParse(obj.ToString(), out outInt))
            {
                return outInt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将Long类型转换为DateTime类型
        /// </summary>
        /// <param name="longstr">long字符</param>
        /// <returns></returns>
        public static DateTime TryDateTime3(long longstr)
        {
            if (longstr <= 0)
                return new DateTime(1990, 1, 1);

            DateTime dtStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1)); //TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(longstr + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);

            return dtResult;
        }

        /// <summary>
        /// 转为byte[]
        /// </summary>
        /// <param name="obj">输入项</param>
        /// <param name="obj">默认值</param>
        /// <returns></returns>
        public static byte[] TryByte(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;

            return (byte[])obj;
        }

        /// <summary>
        ///  转为Bool
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defBl"></param>
        /// <returns></returns>
        public static bool TryBool(object obj, bool defBl)
        {
            if (obj == null || obj == DBNull.Value)
                return defBl;

            if (obj.Equals("0"))
                return false;

            if (obj.Equals("1"))
                return true;

            return bool.Parse(obj.ToString());
        }

        /// <summary>
        ///  转为Bool
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defBl"></param>
        /// <returns></returns>
        public static bool TryBool(object obj)
        {
            return TryBool(obj, false);
        }

        public static System.ComponentModel.TypeConverter GetGrandCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            //if (type == typeof(List<int>))
            //    return new GenericListTypeConverter<int>();
            //if (type == typeof(List<decimal>))
            //    return new GenericListTypeConverter<decimal>();
            //if (type == typeof(List<string>))
            //    return new GenericListTypeConverter<string>();

            //if (type == typeof(Dictionary<int, int>))
            //    return new GenericDictionaryTypeConverter<int, int>();

            return System.ComponentModel.TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                System.ComponentModel.TypeConverter destinationConverter = GetGrandCustomTypeConverter(destinationType);
                System.ComponentModel.TypeConverter sourceConverter = GetGrandCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.GetTypeInfo().IsEnum && value is int)
                    return System.Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }
    }
}
