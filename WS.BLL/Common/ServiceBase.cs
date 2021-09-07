
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Web.MQ;
using WS.DAL.Interface;

namespace WS.BLL
{
    /// <summary>
    /// 业务逻辑层继承此类
    /// </summary>
    public class ServiceBase
    {
        ILogger _logger;

        IConfiguration _configuration;

        IBaseRepository _IBaseRepository;

        IRabbitMQService _rabbitMQService;

        public ServiceBase(IServiceProvider services)
        {
            this.Services = services;
        }

        public IServiceProvider Services { get; set; }

        public IConfiguration Configuration
        {
            get
            {
                if (this._configuration == null)
                {
                    this._configuration = this.Services.GetService(typeof(IConfiguration)) as IConfiguration;
                }

                return this._configuration;
            }
        }

        public IBaseRepository baseRepository
        {
            get
            {
                if (this._IBaseRepository == null)
                {
                    this._IBaseRepository = this.Services.GetService(typeof(IBaseRepository)) as IBaseRepository;
                }

                return this._IBaseRepository;
            }
        }

        public IRabbitMQService  rabbitMQService
        {
            get
            {
                if (this._rabbitMQService == null)
                {
                    this._rabbitMQService = this.Services.GetService(typeof(IRabbitMQService)) as IRabbitMQService;
                }

                return this._rabbitMQService;
            }
        }

        public ILogger Logger
        {
            get
            {
                if (this._logger == null)
                {
                    ILoggerFactory loggerFactory = this.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    ILogger logger = loggerFactory.CreateLogger(this.GetType().FullName);
                    this._logger = logger;
                }

                return this._logger;
            }
        }

        protected virtual void LogException(Exception ex)
        {
            string error = "\r\n 异常类型：" + ex.GetType().FullName + "\r\n 异常源：" + ex.Source + "\r\n 异常位置=" + ex.TargetSite + " \r\n 异常信息=" + ex.Message + " \r\n 异常堆栈：" + ex.StackTrace;

            this.Logger.LogError(error);
        }
    }
}
