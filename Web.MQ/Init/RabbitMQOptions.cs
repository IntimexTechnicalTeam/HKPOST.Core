
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.MQ
{
    public class RabbitMQOptions
    {
        public int Port { get; set; } = 5672;
        public string HostName { get; set; }
        public TimeSpan HeartBeat { get; set; } = new TimeSpan(200);
        public bool AutomaticRecoveryEnabled { get; set; }
        public TimeSpan NetworkRecoveryInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool DispatchConsumersAsync { get; set; } = true;

        public string VirtualHost { get; set; } = "/";
    
    }
}
