using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Web.MQ
{
    /// <summary>
    /// 生产者服务类
    /// </summary>
    public class RabbitMQService : IRabbitMQService
    {      
        //RabbitMQ建议客户端线程之间不要共用Model，至少要保证共用Model的线程发送消息必须是串行的，但是建议尽量共用Connection。       
        ILog _logger = LogManager.GetLogger("system");

        IConnectionChannelPool _channelPool;
        public RabbitMQService(IConnectionChannelPool channelPool)
        {
            //var obj = Activator.CreateInstance(typeof(ConnectionChannelPool));
            this._channelPool = channelPool;
        }

        public void PublishMsg<T>(string queueName, string exchangeName, T t, bool throwIfErrorCaused = true)
        {
            string msg = JsonConvert.SerializeObject(t);
            PublishMsg(queueName, exchangeName, msg, throwIfErrorCaused);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="throwIfErrorCaused"></param>
        public void PublishMsg(string queueName, string exchangeName, string msg, bool throwIfErrorCaused=true)
        {
            var channel = this._channelPool.Rent();

            try
            {
                channel.ExchangeDeclare(exchangeName, type: "direct", durable: true, autoDelete: false);//声明路由
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);//声明队列
                channel.QueueBind(queueName, exchangeName, queueName);//绑定队列与路由

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = Guid.NewGuid().ToString("N");
                //properties.Expiration = "120000";                    //消息过期时间，单位：ms   

                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                properties.Headers = new Dictionary<string, object>
                      {
                        { "key", "value"}
                      };
                var msgBody = Encoding.UTF8.GetBytes(msg);

                channel.ConfirmSelect();
                channel.BasicPublish(exchangeName, routingKey: queueName, basicProperties: properties, body: msgBody);
            }
            catch (Exception ex)
            {
                if (throwIfErrorCaused)
                    throw;

                _logger.Error($"发布消息失败: QueueName-{queueName}, ExchangeName-{exchangeName}, Message-{ex.Message}, StackTrace-{ex.StackTrace}");
            }
            finally
            {
                var returned = this._channelPool.Return(channel);
                if (!returned)
                {
                    channel.Dispose();
                }
            }
        }

        /// <summary>
        /// 发送延时消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="delay">延时时间，单位毫秒</param>
        /// <param name="throwIfErrorCaused"></param>
        public void PublishMsg(string queueName, string exchangeName, string msg, long delay, bool throwIfErrorCaused = true)
        {
            if (delay <= 0)
            {
                this.PublishMsg(queueName, exchangeName, msg, throwIfErrorCaused);
                return;
            }

            this.PublishDeadletterMsg($"{queueName}.deadletter", queueName, exchangeName, msg, delay, throwIfErrorCaused);
        }

        /// <summary>
        /// 发送延时消息
        /// </summary>
        /// <param name="deadLetterQueueName"></param>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="delay">延时时间，单位毫秒</param>
        /// <param name="throwIfErrorCaused"></param>
        public void PublishDelayMsg(string deadLetterQueueName, string queueName, string exchangeName, string msg, long delay, bool throwIfErrorCaused = true)
        {
            this.PublishDeadletterMsg(deadLetterQueueName, queueName, exchangeName, msg, delay, throwIfErrorCaused);
        }

        /// <summary>
        /// 发送死信消息
        /// </summary>
        /// <param name="deadLetterQueueName"></param>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="expire">死信过期时间，单位毫秒</param>
        /// <param name="throwIfErrorCaused"></param>
        public void PublishDeadletterMsg(string deadLetterQueueName, string queueName, string exchangeName, string msg, long expire, bool throwIfErrorCaused = true)
        {
            var channel = this._channelPool.Rent();

            try
            {
                channel.ExchangeDeclare(exchangeName, type:  ExchangeType.Direct, durable: true, autoDelete: false);                //声明路由
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);                                                //声明队列

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("x-expires", 1000 * 60 * 60 * 24);//60秒*60分钟*24小时，路由存在一天

                dic.Add("x-dead-letter-exchange", exchangeName);//过期消息转向路由  
                dic.Add("x-dead-letter-routing-key", queueName);//过期消息转向路由相匹配routingkey  

                //声明队列
                channel.QueueDeclare(queue: deadLetterQueueName,
                    durable: false,//持久性
                    exclusive: false,
                    autoDelete: false,
                    arguments: dic);

                var body = Encoding.UTF8.GetBytes(msg);
                var properties = channel.CreateBasicProperties();               
                properties.Expiration = expire.ToString();

                channel.BasicPublish(exchange: "",
                    routingKey: deadLetterQueueName,
                    basicProperties: properties,
                    body: body);
            }
            catch (Exception ex)
            {
                if (throwIfErrorCaused)
                    throw;

                _logger.Error($"发布消息失败: DeadLetterQueueName-{deadLetterQueueName}, QueueName-{queueName}, ExchangeName-{exchangeName}, Message-{ex.Message}, StackTrace-{ex.StackTrace}");
            }
            finally
            {
                var returned = this._channelPool.Return(channel);
                if (!returned)
                {
                    channel.Dispose();
                }
            }
        }

    }
}
