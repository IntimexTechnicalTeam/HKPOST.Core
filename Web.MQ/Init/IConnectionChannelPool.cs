using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.MQ
{
    public interface IConnectionChannelPool
    {
        IConnection GetConnection();

        IModel Rent();

        bool Return(IModel context);
    }
}
