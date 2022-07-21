using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;

namespace Web.MQ
{
    public interface IRabbitMQService //: IDependency
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="throwIfErrorCaused"></param>
        void PublishMsg(string queueName, string exchangeName, string msg, bool throwIfErrorCaused = true);
        /// <summary>
        /// 发送延时消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="delay">延时时间，单位毫秒</param>
        /// <param name="throwIfErrorCaused"></param>
        void PublishMsg(string queueName, string exchangeName, string msg, long delay, bool throwIfErrorCaused = true);
        /// <summary>
        /// 发送延时消息
        /// </summary>
        /// <param name="deadLetterQueueName"></param>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="msg"></param>
        /// <param name="delay">延时时间，单位毫秒</param>
        /// <param name="throwIfErrorCaused"></param>
        void PublishDelayMsg(string deadLetterQueueName, string queueName, string exchangeName, string msg, long delay, bool throwIfErrorCaused = true);
    }
}
