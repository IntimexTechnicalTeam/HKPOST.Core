using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtension
    {
        public static void LogMsg(this ILogger logger, string msg)
        {
            logger.LogError(msg);
        }

        public static void LogException(this ILogger logger, Exception ex)
        {
            logger.LogException(ex, 0);
        }
        static void LogException(this ILogger logger, Exception ex, int nest)
        {
            string error = "\r\n 异常类型：" + ex.GetType().FullName + "\r\n 异常源：" + ex.Source + "\r\n 异常位置=" + ex.TargetSite + " \r\n 异常信息=" + ex.Message + " \r\n 异常堆栈：" + ex.StackTrace;

            logger.LogError(error);

            if (nest < 3 && ex.InnerException != null)
            {
                logger.LogException(ex.InnerException, nest++);
            }
        }
    }
}
