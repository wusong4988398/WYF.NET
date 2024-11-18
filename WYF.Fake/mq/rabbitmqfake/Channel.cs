using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Fake.mq.rabbitmqfake.exchange;
using WYF.Fake.mq.rabbitmqfake.queue;

namespace WYF.Fake.mq.rabbitmqfake
{
    public static class Channel
    {
        /// <summary>
        /// 声明一个队列。
        /// </summary>
        /// <param name="exchangeName">交换机名称。</param>
        /// <param name="queueName">队列名称，如果为null，则表示广播模式。</param>
        public static void QueueDeclare(string exchangeName, string queueName)
        {
            ExchangeEngine.ExchangeDeclare(exchangeName);
            Exchange exchange = ExchangeEngine.GetExchange(exchangeName);

            if (queueName != null)
            {
                exchange.IsBroadcast = false;

                exchange.InitQueue(queueName, new QueueFake());
            }
            else
            {
                exchange.IsBroadcast = true;
            }
        }

        /// <summary>
        /// 开始消费消息。
        /// </summary>
        /// <param name="exchangeName">交换机名称。</param>
        /// <param name="queueName">队列名称。</param>
        /// <param name="consumer">消费者对象。</param>
        public static void BasicConsume(string exchangeName, string queueName, IConsume consumer)
        {
            Exchange exchange = ExchangeEngine.GetExchange(exchangeName);
            if (exchange == null)
            {
                throw new Exception("exchange hasn't been declared");
            }

            if (exchange.IsBroadcast)
            {
                string broadcastQueueName = "broadcast-" + consumer.GetHashCode();
                QueueFake q = new QueueFake();
                exchange.InitQueue(broadcastQueueName, q);
                q.SetConsumer(consumer);
            }
            else
            {
                QueueFake q = exchange.GetQueue(queueName);
                if (q == null)
                {
                    throw new Exception("queue hasn't been declared: ");
                }
                q.SetConsumer(consumer);
            }
        }

        /// <summary>
        /// 发布消息。
        /// </summary>
        /// <param name="exchangeName">交换机名称。</param>
        /// <param name="queueName">队列名称，可以为null。</param>
        /// <param name="message">要发送的消息。</param>
        public static void Publish(string exchangeName, string queueName, object message)
        {
            ExchangeEngine.Publish(exchangeName, queueName, message);
        }
    }
}
