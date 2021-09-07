using log4net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Web.MQ
{
    /// <summary>
    /// channel连接池,因为生产者可能有并发(队列太多)，所以用ConcurrentQueue来处理channel
    /// </summary>
    public class ConnectionChannelPool : IConnectionChannelPool, IDisposable
    {
        private const int DefaultPoolSize = 15;
        private readonly Func<IConnection> _connectionActivator;
        private readonly ILogger<ConnectionChannelPool> _logger;
        private readonly ConcurrentQueue<IModel> _pool;
        private IConnection _connection;

        RabbitMQOptions _rabbitMQOptions;

        private static readonly object SLock = new object();

        private int _count;
        private int _maxSize;

        public ConnectionChannelPool(
            ILogger<ConnectionChannelPool> logger,
            IOptions<RabbitMQOptions> optionsAccessor)
        {
            _logger = logger;
            _maxSize = DefaultPoolSize;
            _pool = new ConcurrentQueue<IModel>();

            this._rabbitMQOptions = optionsAccessor.Value;

            var options = optionsAccessor.Value;

            _connectionActivator = CreateConnection(options);
        }

        IModel IConnectionChannelPool.Rent()
        {
            lock (SLock)
            {
                while (_count > _maxSize)
                {
                    Thread.SpinWait(1);
                }
                return Rent();
            }
        }

        bool IConnectionChannelPool.Return(IModel connection)
        {
            return Return(connection);
        }

        public IConnection GetConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return _connection;
            }

            this._logger.LogError($"RabbitMQHostName: {this._rabbitMQOptions.HostName}");

            _connection = _connectionActivator();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            return _connection;
        }

        public void Dispose()
        {
            _maxSize = 0;

            while (_pool.TryDequeue(out var context))
            {
                context.Dispose();
            }

            if (this._connection != null)
            {
                this._connection.Dispose();
                this._connection = null;
            }
        }

        private static Func<IConnection> CreateConnection(RabbitMQOptions options)
        {
            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name.ToLower();

            var factory = new ConnectionFactory
            {
                //HostName = options.HostName,
                UserName = options.UserName,
                Port = options.Port,
                Password = options.Password,
                VirtualHost = "/",
                AutomaticRecoveryEnabled = true
            };

            if (options.HostName.Contains(","))
            {
                return () => factory.CreateConnection(
                    options.HostName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries), serviceName);
            }

            factory.HostName = options.HostName;

            return () => factory.CreateConnection(serviceName);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning($"RabbitMQ client connection closed! --> {e.ReplyText}");
        }

        public virtual IModel Rent()
        {
            if (_pool.TryDequeue(out var model))
            {
                Interlocked.Decrement(ref _count);

                Debug.Assert(_count >= 0);
                return model;
            }

            try
            {
                model = GetConnection().CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("\r\n 出现异常类型：" + ex.GetType().FullName + "\r\n 异常源：" + ex.Source + "\r\n 异常位置=" + ex.TargetSite + " \r\n 异常信息=" + ex.Message + " \r\n 异常堆栈：" + ex.StackTrace);
                Console.WriteLine(ex);
                throw;
            }

            return model;
        }

        public virtual bool Return(IModel connection)
        {
            if (connection.IsClosed || !connection.IsOpen)
            {
                return false;
            }

            if (Interlocked.Increment(ref _count) <= _maxSize)
            {
                _pool.Enqueue(connection);

                return true;
            }

            Interlocked.Decrement(ref _count);

            Debug.Assert(_maxSize == 0 || _pool.Count <= _maxSize);

            return false;
        }
    }

}
