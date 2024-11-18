using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Fake.mq.rabbitmqfake.queue;

namespace WYF.Fake.mq.rabbitmqfake.exchange
{
    public class Exchange
    {
        private bool _isBroadcast = false;

        /// <summary>
        /// 获取或设置是否为广播模式。
        /// </summary>
        public bool IsBroadcast
        {
            get { return _isBroadcast; }
            set { _isBroadcast = value; }
        }

        private readonly ConcurrentDictionary<string, QueueFake> _queues = new ConcurrentDictionary<string, QueueFake>();

        /// <summary>
        /// 初始化队列。
        /// </summary>
        /// <param name="queueName">队列名称。</param>
        /// <param name="queueFake">队列对象。</param>
        public void InitQueue(string queueName, QueueFake queueFake)
        {
            if (!_queues.ContainsKey(queueName))
            {
                _queues[queueName] = queueFake;
            }
        }

        /// <summary>
        /// 获取指定名称的队列。
        /// </summary>
        /// <param name="queueName">队列名称。</param>
        /// <returns>队列对象。</returns>
        public QueueFake GetQueue(string queueName)
        {
            _queues.TryGetValue(queueName, out QueueFake queue);
            return queue;
        }

        /// <summary>
        /// 获取所有队列。
        /// </summary>
        /// <returns>队列集合。</returns>
        public ICollection<QueueFake> GetQueues()
        {
            return _queues.Values;
        }
    }
}
