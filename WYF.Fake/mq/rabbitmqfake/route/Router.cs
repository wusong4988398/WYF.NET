using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Fake.mq.rabbitmqfake.exchange;
using WYF.Fake.mq.rabbitmqfake.queue;

namespace WYF.Fake.mq.rabbitmqfake.route
{
    public static class Router
    {
        public static QueueFake[] Route(Exchange exchange, string queueName)
        {
            //如果交换机是广播模式（IsBroadcast 为 true），则获取所有队列。
            if (exchange.IsBroadcast)
            {
                var queues = exchange.GetQueues();
                if (queues != null)
                {
                    return queues.ToArray();
                }
                return new QueueFake[0];
            }
            return new QueueFake[] { exchange.GetQueue(queueName) };
        }
    }
}
