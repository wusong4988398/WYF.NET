using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Fake.mq.rabbitmqfake.queue;
using WYF.Fake.mq.rabbitmqfake.route;

namespace WYF.Fake.mq.rabbitmqfake.exchange
{
    /// <summary>
    /// 用于管理交换机，提供声明交换机、获取交换机和发布消息的方法。
    /// </summary>
    public class ExchangeEngine
    {
        private static readonly ConcurrentDictionary<string, Exchange> Exchanges = new ConcurrentDictionary<string, Exchange>();

        /// <summary>
        /// 声明一个交换机。
        /// </summary>
        /// <param name="exchange">交换机名称。</param>
        public static void ExchangeDeclare(string exchange)
        {
            if (exchange == null)
            {
                throw new Exception("exchange cannot be null");
            }

            if (!Exchanges.ContainsKey(exchange))
            {
                lock (typeof(ExchangeEngine))
                {
                    if (!Exchanges.ContainsKey(exchange))
                    {
                        Exchanges[exchange] = new Exchange();
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定名称的交换机。
        /// </summary>
        /// <param name="exchangeName">交换机名称。</param>
        /// <returns>交换机对象。</returns>
        public static Exchange GetExchange(string exchangeName)
        {
            Exchanges.TryGetValue(exchangeName, out Exchange exchange);
            return exchange;
        }

        /// <summary>
        /// 发布消息到指定的交换机和队列。
        /// </summary>
        /// <param name="exchangeName">交换机名称。</param>
        /// <param name="queueName">队列名称，可以为null。</param>
        /// <param name="message">要发送的消息。</param>
        public static void Publish(string exchangeName, string queueName, object message)
        {
            Exchange exchange = GetExchange(exchangeName);
            if (exchange != null)
            {
                QueueFake[] queues = Router.Route(exchange, queueName);
                foreach (QueueFake queue in queues)
                {
                    queue.Enqueue(message);
                }
            }
        }
    }
}
