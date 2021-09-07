using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Framework
{
    public class DateTimeHelp
    {
        /// <summary>
        /// DateTime转换为Unix时间戳（秒数）
        /// </summary>
        /// <param name="dt">dateTime时间</param>
        /// <returns></returns>
        public static long ToUnixTimeSeconds(DateTime dt)
        {
            /* DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));*/ // 当地时区
                                                                                                             //DateTime serverTime2 = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);//将时间转换为特定时区中的时间。
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);//将协调的通用时间（UTC）转换为指定时区中的时间。
            long timeStamp = (long)(dt - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        /// <summary>
        /// DateTime转换为Unix时间戳（毫秒数13位）
        /// </summary>
        /// <param name="dt">dateTime时间</param>
        /// <returns></returns>
        public static long ToUnixTimeMilliseconds(DateTime dt)
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);//将协调的通用时间（UTC）转换为指定时区中的时间。
            long timeStamp = (long)(dt - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }

        /// <summary>
        /// 当前时间戳
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long GetTimeStamp(int length = 13)
        {
            long t = long.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            if (length == 13) { t = (DateTime.Now.Ticks - startTime.Ticks) / 10000; }     //除10000调整为13位 
            if (length == 10) { t = (DateTime.Now.Ticks - startTime.Ticks) / 10000000; }   //除10000000调整为10位 
            return t;
        }

        /// <summary>
        /// Unix时间戳（秒数）转换为DateTime
        /// </summary>
        /// <param name="unixTimeStamp">Unix时间戳（秒数）</param>
        /// <returns></returns>
        public static DateTime UnixTimeSecondsConvDateTime(long unixTimeStamp)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }



        /// <summary>
        /// 时间戳（毫秒数）转换为DateTime
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（毫秒数）</param>
        /// <returns></returns>
        public static DateTime UnixTimeMillisecondsConvDateTime(long unixTimeStamp)
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);//将协调的通用时间（UTC）转换为指定时区中的时间。
            DateTime dt = startTime.AddMilliseconds(unixTimeStamp);
            return dt;
        }
        ///// <summary>
        ///// 友好显示时间
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string FriendlyDisplayTime(DateTime value)
        //{
        //    string result = "";
        //    double temp = (DateTime.Now - value).TotalHours;
        //    if (temp <= 24)
        //    {
        //        result = value.ToString("HH:mm");
        //    }
        //    else if (temp > 24)
        //    {
        //        result = value.ToString("yyyy-MM-dd");
        //    }
        //    //if(value>DateTime.Now())
        //    return result;
        //}

        /// <summary>
        /// 根据日期返回星期几
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string CaculateWeekDay(DateTime dt)
        {

            int y = dt.Year;
            int m = dt.Month;
            int d = dt.Day;
            string[] weekstr = { "日", "一", "二", "三", "四", "五", "六" };

            if (m < 3)
            {
                m += 12;
                if (y % 400 == 0 || y % 100 != 0 && y % 4 == 0)
                {
                    d--;
                }
            }
            else
            {
                d += 1;
            }
            return "周" + weekstr[(d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7];


        }

        /// <summary>
        /// 星期几转数字
        /// </summary>
        /// <param name="strWeek"></param>
        /// <returns></returns>
        public static int FormatMatchWeek(string strWeek)
        {
            int intWeek = 0;
            switch (strWeek)
            {
                case "周一":
                    intWeek = 1;
                    break;
                case "周二":
                    intWeek = 2;
                    break;
                case "周三":
                    intWeek = 3;
                    break;
                case "周四":
                    intWeek = 4;
                    break;
                case "周五":
                    intWeek = 5;
                    break;
                case "周六":
                    intWeek = 6;
                    break;
                case "周日":
                    intWeek = 7;
                    break;
            }
            return intWeek;
        }

        #region 时间处理

        /// <summary>
        /// 截止时间
        /// </summary>
        /// <param name="matchDate"></param>
        /// <returns></returns>
        public static string GetStopSellTime(string matchDate)
        {
            string stopSellTime = "";
            string week = Convert.ToDateTime(matchDate).ToString("dddd");
            string hh = Convert.ToDateTime(matchDate).ToString("HH");
            switch (week)
            {
                case "星期二":
                case "星期三":
                case "星期四":
                case "星期五":
                case "星期六":
                    if (int.Parse(hh) <= 9)
                    {
                        stopSellTime = Convert.ToDateTime(matchDate).AddDays(-1).ToString("yyyy-MM-dd") + " 23:55:00";
                    }
                    else
                    {
                        stopSellTime = Convert.ToDateTime(matchDate).AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    break;
                case "星期日":
                case "星期一":
                    if (int.Parse(hh) >= 1 && int.Parse(hh) <= 9)
                    {
                        stopSellTime = Convert.ToDateTime(matchDate).ToString("yyyy-MM-dd") + " 00:55:00";
                    }
                    else
                    {
                        stopSellTime = Convert.ToDateTime(matchDate).AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    break;
            }
            return stopSellTime;

            //return Convert.ToDateTime(matchDate).AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回周几
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static string GetWeek(string week)
        {
            string res = "";
            switch (week)
            {
                case "1":
                    res = "周一";
                    break;
                case "2":
                    res = "周二";
                    break;
                case "3":
                    res = "周三";
                    break;
                case "4":
                    res = "周四";
                    break;
                case "5":
                    res = "周五";
                    break;
                case "6":
                    res = "周六";
                    break;
                case "7":
                    res = "周日";
                    break;
                case "0":
                    res = "周日";
                    break;
            }
            return res;
        }

        /// <summary>
        /// 根据日期返回字符串的星期几
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetWeek(DateTime dt)
        {
            string result = "";

            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    result = "周五";
                    break;
                case DayOfWeek.Monday:
                    result = "周一";
                    break;
                case DayOfWeek.Saturday:
                    result = "周六";
                    break;
                case DayOfWeek.Sunday:
                    result = "周日";
                    break;
                case DayOfWeek.Thursday:
                    result = "周四";
                    break;
                case DayOfWeek.Tuesday:
                    result = "周二";
                    break;
                case DayOfWeek.Wednesday:
                    result = "周三";
                    break;
            }

            return result;
        }
        #endregion

        #region 时间友好显示
        /// <summary>
        /// 友好显示时间(多少时间以前)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FriendlyShowTime(DateTime date)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;
            TimeSpan ts = DateTime.Now - date;
            double delta = ts.TotalSeconds;
            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * minute)
            {
                return ts.Seconds == 1 ? "1秒前" : ts.Seconds + "秒前";
            }
            if (delta < 2 * minute)
            {
                return "1分钟之前";
            }
            if (delta < 45 * minute)
            {
                return ts.Minutes + "分钟";
            }
            if (delta < 90 * minute)
            {
                return "1小时前";
            }
            if (delta < 24 * hour)
            {
                return ts.Hours + "小时前";
            }
            if (delta < 48 * hour)
            {
                return "昨天";
            }
            if (delta < 30 * day)
            {
                return ts.Days + " 天之前";
            }
            if (delta < 12 * month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "一个月之前" : months + "月之前";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "一年前" : years + "年前";
        }

        /// <summary>
        /// 剩余时间友好显示
        /// </summary>
        /// <param name="date">结束时间</param>
        /// <returns></returns>
        public static string GetRemainingTime(DateTime date)
        {
            TimeSpan ts = date - DateTime.Now;
            string res;
            double delta = ts.TotalHours;
            // ts = endDate.Subtract(endDate);
            if (delta > 1)
            {
                res = "剩余" + ts.Days + "天" + ts.Hours + "小时";//+ ts.Minutes.ToString() + "分钟";
            }
            else
            {
                res = "剩余1小时以内";
            }
            return res;
        }

        //只格式化2天内的时间
        public static string AgoDateFomat(DateTime date)
        {
            var times = DateTime.Now - date;
            var s = Convert.ToDecimal(times.TotalSeconds);//秒
            var m = Convert.ToDecimal(times.TotalMinutes);//分钟
            var h = Convert.ToDecimal(times.TotalHours);//小时
            var d = Convert.ToDecimal(times.TotalDays);//天

            return s < 60 ? "" + decimal.Truncate(s) + " 秒前" : m < 60 ? "" + decimal.Truncate(m) + " 分钟前" : h < 24 ? "" + decimal.Truncate(h) + " 小时前" : d < 2 ? "" + decimal.Truncate(d) + " 天前" : date.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion
    }
}

