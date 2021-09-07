using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace Web.Quartz
{
    public abstract class JobBase : IJob
    {
        public JobBase(IServiceProvider services)
        {
            this.Services = services;
            this.Logger = services.GetService<ILoggerFactory>().CreateLogger(this.GetType().FullName);
        }

        public IServiceProvider Services { get; set; }
        public ILogger Logger { get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            this.Logger.LogError($"{this.GetType().Name} starting...");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                await this.ExecuteImpl();
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"{this.GetType().Name} error...");
                //this.Logger.LogException(ex);
            }
            stopWatch.Stop();
            this.Logger.LogError($"{this.GetType().Name} end，用时{stopWatch.Elapsed.TotalMilliseconds}毫秒");
        }

        protected abstract Task ExecuteImpl();
    }

}
