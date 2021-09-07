using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Quartz
{
    /// <summary>
    /// Cron表达式服务基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CronJobHostServiceBase<T> : BackgroundService where T : IJob
    {
        IServiceProvider _services;
        IScheduler _scheduler;
        ILogger _logger;
        
        public CronJobHostServiceBase(IServiceProvider services)
        {
            this._services = services;
            this._scheduler = services.GetService<IScheduler>();
            this._logger = services.GetService<ILoggerFactory>().CreateLogger(this.GetType().FullName);
        }

        public abstract string JobCron { get; }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            string jobName = typeof(T).Name;

            this._logger.LogError($"{jobName} starting...");

            IJobDetail job = JobBuilder.Create<T>()
                    .WithIdentity(jobName, jobName)
                    .Build();

            ICronTrigger cronTrigger = (ICronTrigger)TriggerBuilder.Create()
                                   .WithIdentity(Guid.NewGuid().ToString(), jobName)
                                   .WithCronSchedule(this.JobCron)
                                   .Build();
            await _scheduler.ScheduleJob(job, cronTrigger);

            //ITrigger cronTrigger = TriggerBuilder.Create()
            //    .WithIdentity(jobName, jobName).StartNow()
            //                             .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever())
            //                             .Build();
            //await _scheduler.ScheduleJob(job, cronTrigger);

            await _scheduler.Start(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogError($"{typeof(T).Name} 定时任务停止");
            await _scheduler.Shutdown(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }
    }

}
