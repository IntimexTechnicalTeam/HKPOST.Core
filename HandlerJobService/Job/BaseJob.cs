
using Microsoft.Extensions.Logging;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Web.Framework;

namespace HandleJobService
{
    public abstract class BaseJob :IJob 
    {
        public ILogger _logger { get; set; }

        public IServiceProvider services { get; set; }

        /// <summary>
        /// 具体业务逻辑方法
        /// </summary>
        /// <returns></returns>
        protected abstract Task<SystemResult> Handle();

        /// <summary>
        /// Job名称
        /// </summary>
        public abstract string JobName { get; }

        /// <summary>
        /// 具体的日志类名
        /// </summary>
        public abstract string categoryName { get; }

        public BaseJob(IServiceProvider services)
        {
            this.services = services;
            this._logger = services.GetService<ILoggerFactory>().CreateLogger(categoryName);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {               
                var result =  await Handle();
                //if (result.error != 0)
                //    _logger.LogError("只记录出错日志");

                SaveLog(result.Message, result.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError($"\r\n 处理{JobName}定时任务 异常类型：" + JobName.GetType().FullName + "\r\n 异常源：" + ex.Source + "\r\n 异常位置=" + ex.TargetSite + " \r\n 异常信息=" + ex.Message + " \r\n 异常堆栈：" + ex.StackTrace);
            }
            stopWatch.Stop();
        }

        public void SaveLog(string msg, bool flag)
        {
            if (flag)
                _logger.LogTrace(msg);
            else
                _logger.LogError(msg);
            Console.WriteLine(msg);
        }
    }
}
