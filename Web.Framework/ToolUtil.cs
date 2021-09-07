using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Web.Framework
{
    public class ToolUtil
    {

        /// <summary>
        /// 防SQL注入 过滤掉',;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SqlMisplaced(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            string res = value.Replace("'", "").Replace(";", "");
            return res;
        }

        /// <summary>
        /// 去除字符串中多个相同的字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="count">出现次数</param>
        public static string TrimXT(string str, int count)
        {
            string result = "";
            for (int i = 0; i < 10; i++)
            {
                if (GetCharInStringCount(i.ToString(), str) == count)
                {
                    str = str.Replace(i.ToString(), "");
                }
            }

            result = str;
            return result;
        }

        /// <summary>
        /// 返回字符串在字符串中出现的次数
        /// </summary>
        /// <param name="Char">要检测出现的字符</param>
        /// <param name="String">要检测的字符串</param>
        /// <returns>出现次数</returns>

        private static int GetCharInStringCount(string Char, string String)
        {
            string str = String.Replace(Char, "");
            return (String.Length - str.Length) / Char.Length;
        }


        /// <summary>
        /// 替换手机号码号中间信息（前7后4,中间信息以newValue字符代替）
        /// </summary>
        /// <param name="str">手机号码</param>
        /// <param name="newValue">中间信息符换字符</param>
        /// <returns></returns>
        public static string GetCardInfo(string str, string newValue)
        {
            string temp = string.Empty;
            temp = str.Substring(0, 4);
            Char[] tempChar = str.ToCharArray(4, str.Length - 8);
            for (int i = 0; i < tempChar.Length; i++)
            {
                temp += newValue;
            }
            temp += str.Substring(str.Length - 4);
            return temp;
        }




        /// <summary>
        /// 正数四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double Round(double value, int decimals)
        {
            //Math.Ceiling
            if (value < 0)
            {
                return Math.Round(value + 5 / Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero);
            }
            else
            {
                return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
            }
        }
        /// <summary>
        /// 正数四舍五入
        /// </summary>
        /// <param name="d"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal Round(decimal d, int decimals)
        {
            if (d < 0)
            {
                return Math.Round(d + 5 / (decimal)Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero);
            }
            else
            {
                return Math.Round(d, decimals, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 网络路径转本地路径
        /// </summary>
        /// <param name="imagesurl"></param>
        /// <returns></returns>
        public static string UrlTolocal(string imagesurl)
        {
            //string tmpRootDir = HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            Uri baseUri = new Uri(imagesurl);

            //string imagesurl2 = tmpRootDir + baseUri.AbsolutePath;
            string imageurl = baseUri.AbsolutePath;
            return imageurl;
        }


        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="startNumber"></param>
        /// <param name="endNumber"></param>
        /// <returns></returns>
        public static string GetRandom(int startNumber, int endNumber)
        {
            Random random = new Random();
            return random.Next(startNumber, endNumber).ToString();
        }

        /// <summary>
        /// 概率
        /// </summary>
        /// <param name="prob"></param>
        /// <returns></returns>
        public static int GetPR(float[] prob)
        {
            int result = 0;
            int n = (int)(prob.Sum() * 1000);           //计算概率总和，放大1000倍
            Random rnd = new Random();
            float x = (float)rnd.Next(0, n) / 1000;       //随机生成0~概率总和的数字

            for (int i = 0; i < prob.Count(); i++)
            {
                float pre = prob.Take(i).Sum();         //区间下界
                float next = prob.Take(i + 1).Sum();    //区间上界
                if (x >= pre && x < next)               //如果在该区间范围内，就返回结果退出循环
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        #region HTML处理
        /// <summary>
        /// 提取HTML代码中文字
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string StripHtml(string strHtml)
        {
            string[] aryReg =
                {
                  @"<script[^>]*?>.*?</script>",
                  @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", @"([\r\n])[\s]+",
                  @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);",
                  @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);",
                  @"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n"
                };

            string[] aryRep =
            {
              "", "", "", "\"", "&", "<", ">", "   ", "\xa1",  //chr(161),
              "\xa2",  //chr(162),
              "\xa3",  //chr(163),
              "\xa9",  //chr(169),
              "", "\r\n", ""
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        ///<summary>
        ///去除HTML标记
        ///</summary>
        ///<param name="NoHtml">包括HTML的源码   </param>
        ///<returns>已经去除后的文字</returns>
        public static string NoHtml(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",
            RegexOptions.IgnoreCase);
            //删除HTML 
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"–>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!–.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring = Htm HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            Htmlstring = Htmlstring.Trim();
            return Htmlstring;
        }


        ///<summary>
        ///取出文本中的图片地址
        ///</summary>
        ///<param name="strHtml">HTMLStr</param>
        public static string GetImgUrl(string strHtml)
        {
            string str = string.Empty;
            // string sPattern = @"^<img\s+[^>]*>";
            Regex r = new Regex(@"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>",
              RegexOptions.Compiled);
            Match m = r.Match(strHtml.ToLower());
            if (m.Success)
                str = m.Result("${url}");
            return str;
        }

        /// <summary>
        /// 检查是否有Html标签
        /// </summary>
        /// <param name="html">Html源码</param>
        /// <returns>存在为True</returns>
        public static bool CheckHtml(string html)
        {
            string filter = "<[\\s\\S]*?>";

            if (Regex.IsMatch(html, filter))
            {
                return true;
            }
            filter = "[<>][\\s\\S]*?";
            if (Regex.IsMatch(html, filter))
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 判断是否是手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            Regex regex = new Regex("^1[3456789]\\d{9}$");
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 检查一个用户名是否合法。
        /// </summary>
        /// <param name="accountName"></param>
        public static void EnsureAccountNameLegal(string accountName, int minLength = 6, int maxLength = 20)
        {
            if (accountName == null)
                throw new ArgumentNullException();

            accountName = accountName.Trim();

            if (accountName.Length < minLength || accountName.Length > maxLength)
                throw new InvalidInputException($"用户名长度必须为 {minLength}-{maxLength} 位");

            string illegalChar;
            if (HasIllegalChar(accountName, out illegalChar))
                throw new InvalidInputException($"用户名包含非法字符{illegalChar}");
        }
        /// <summary>
        /// 判断用户名中是否包含非法字符。如果包含非法字符，则返回false。
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="illegalChar"></param>
        /// <returns></returns>
        public static bool HasIllegalChar(string accountName, out string illegalChar)
        {
            /*
             * 
             */

            //[^a-z]

            illegalChar = null;

            if (string.IsNullOrEmpty(accountName))
                throw new ArgumentException();

            Match match = Regex.Match(accountName, @"[^A-Za-z0-9_\.-]");
            if (match == null || match.Success == false)
                return false;

            illegalChar = match.Value;
            return true;
        }

        /// <summary>
        /// 替换部分字符串为星号
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string StrReplaceWithStar(string userName, bool? flag = null)
        {
            string left = string.Empty;
            string right = string.Empty;
            string middle = string.Empty;
            string newStr = userName;
            int nameLength = userName.Length;

            if (flag == null || flag == false)
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = "*";
                }

                if (nameLength <= 6)
                {
                    left = "****";
                    right = ToRight(userName, 2);
                }
                else if (nameLength == 7)
                {
                    left = "****";
                    right = ToRight(userName, 3);
                }
                else if (nameLength == 8)
                {
                    left = "*****";
                    right = ToRight(userName, 3);
                }
                else if (nameLength == 9)
                {
                    left = "******";
                    right = ToRight(userName, 3);
                }
                else if (nameLength == 10)
                {
                    left = "*******";
                    right = ToRight(userName, 3);
                }
                else if (nameLength >= 11)
                {
                    left = "*******";
                    right = ToRight(userName, 4);
                }

                newStr = string.Join("", left, right);
            }

            return newStr;
        }

        public static string ToRight(string str, int length)
        {
            return string.Join("", string.Join("", str.Reverse()).Substring(0, length).Reverse());
        }
    }
}
