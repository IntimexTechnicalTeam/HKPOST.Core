using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Web.Framework;

namespace Web.MQ
{
    /// <summary>
    /// 消费者服务基类
    /// </summary>
    public abstract class ConsumerHostServiceBase : BackgroundService
    {
        IConnectionFactory _mqFactory;
        IConnection _mqConnection;
        IModel _mqChannel;

        //处理连续错误次数
        int _consecutiveHandleFailedCount = 0;

        List<double> _useTimes = new List<double>();

        protected ConsumerHostServiceBase(IServiceProvider services)
        {
            this.Services = services;        
            this.Logger = services.GetService<ILoggerFactory>().CreateLogger(categoryName);
            this._mqFactory = this.InitConnectionFactory();
            this._mqConnection = this._mqFactory.CreateConnection();
            this._mqChannel = this._mqConnection.CreateModel();
        }

        public IServiceProvider Services { get; set; }
        public ILogger Logger { get; set; }

        ConnectionFactory InitConnectionFactory()
        {
            var config = GetMQConfig();

            ConnectionFactory rabbitMqFactory = new ConnectionFactory
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password,
                VirtualHost = config.VirtualHost,
                AutomaticRecoveryEnabled = true,// 设置断线自动恢复
                DispatchConsumersAsync = true               //开启异步消费
            };

            return rabbitMqFactory;
        }

        protected virtual RabbitMQOptions GetMQConfig()
        {
            RabbitMQOptions config = new RabbitMQOptions();

            config.HostName = Globals.Configuration["RabbitMQ:HostName"];
            config.UserName = Globals.Configuration["RabbitMQ:UserName"];
            config.Password = Globals.Configuration["RabbitMQ:Password"];
            config.VirtualHost = "/";

            int port;
            if (!int.TryParse(Globals.Configuration["RabbitMQ:Port"], out port))
            {
                port = 5672;
            }

            config.Port = port;

            return config;
        }

        /// <summary>
        /// 消息处理失败是否将消息返回到队列中去，默认 false
        /// </summary>
        protected virtual bool RejectIfHandleFailed { get { return false; } }
        protected abstract string Queue { get; }
        protected virtual string Exchange { get { return this.Queue; } }

        protected abstract string categoryName { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"*****************{this.GetType().Name}正在启动******************");
            this.Logger.LogError($"*****************{this.GetType().Name}正在启动******************");

            IModel channel = this._mqChannel;
            string queue = this.Queue;
            string exchange = this.Exchange;
            string routingKey = this.Queue;

            channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            channel.ExchangeDeclare(exchange: exchange, type: "direct", durable: true, autoDelete: false, arguments: null);    //交换类型
            channel.QueueBind(queue: queue,
                           exchange: exchange,
                           routingKey: routingKey);

            //公平分发 同一时间只处理一个消息
            channel.BasicQos(0, 1, true);

            //异步处理
            var consumer = new AsyncEventingBasicConsumer(channel);
 
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            consumer.Received += OnConsumerReceived;

            channel.BasicConsume(queue, false, consumer);//返回通知才删除队列

            Console.WriteLine($"*****************{this.GetType().Name}启动成功******************");
            this.Logger.LogError($"*****************{this.GetType().Name}启动成功******************");

            await Task.Delay(1);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this._mqChannel.Close();
            this._mqChannel.Close();
            await base.StopAsync(cancellationToken);
        }

        protected abstract Task Handle(string msg);

        async Task OnConsumerReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            string msg = Encoding.UTF8.GetString(body);
            this.Logger.LogDebug("接收到消息msg=" + msg);

            try
            {
                double useTime = await TraceTime(async () =>
                {
                    await this.Handle(msg);
                });

                this._useTimes.Add(useTime);

                this._mqChannel.BasicAck(ea.DeliveryTag, multiple: false);
                this._consecutiveHandleFailedCount = 0;

                if (_useTimes.Count >= 50)
                {
                    dynamic stat = new ExpandoObject();
                    stat.TotalUseTime = this._useTimes.Sum();
                    stat.MaxUseTime = this._useTimes.Max();
                    stat.MinUseTime = this._useTimes.Min();
                    stat.AvgUseTime = this._useTimes.Average();
                    stat.UseTimes = this._useTimes;
                    this.Logger.LogDebug($"消息处理统计：{JsonConvert.SerializeObject(stat)}");

                    this._useTimes.Clear();
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"处理失败 {this.Queue}：[{msg}]，异常={ex.Message} {ex.StackTrace}");
                if (this.RejectIfHandleFailed)
                {
                    this._mqChannel.BasicReject(ea.DeliveryTag, true);

                    this._consecutiveHandleFailedCount++;
                    if (this._consecutiveHandleFailedCount >= 20)
                    {
                        Thread.Sleep(10 * 1000); //休息10秒，防止意外情况，还无限重试
                        this._consecutiveHandleFailedCount = 0;
                    }
                }
                else
                {
                    this._mqChannel.BasicAck(ea.DeliveryTag, multiple: false);
                }
            }
        }
        protected virtual Task OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            this.Logger.LogInformation($"消息队列 consumer registered {e.ConsumerTags}");
            return Task.CompletedTask;
        }
        protected virtual Task OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            this.Logger.LogInformation($"消息队列 consumer unregistered {e.ConsumerTags}");
            return Task.CompletedTask;
        }
        protected virtual Task OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            this.Logger.LogInformation($"消息队列 consumer cancelled {e.ConsumerTags}");
            return Task.CompletedTask;
        }
        protected virtual Task OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            this._mqChannel.Dispose();
            this._mqConnection.Dispose();
            base.Dispose();
        }

        static async Task<double> TraceTime(Func<Task> func)
        {
            DateTime startTime = DateTime.Now;
            await func();
            var useTime = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            return useTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="flag">true写到Trace，false写到Error</param>
        public void SaveLog(string msg, bool flag=true)
        {
            if (flag)
                Logger.LogInformation(msg);
            else
                Logger.LogError(msg);
            Console.WriteLine(msg);
        }

    }
}
