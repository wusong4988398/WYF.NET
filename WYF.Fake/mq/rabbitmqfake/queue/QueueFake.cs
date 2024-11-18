using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WYF.Fake.mq.rabbitmqfake.queue
{
    public class QueueFake
    {
        private readonly ConcurrentQueue<object> _queue = new ConcurrentQueue<object>();

        private static readonly TaskFactory _taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(
            Environment.GetEnvironmentVariable("queuefake.thread.pool.max") != null ?
            int.Parse(Environment.GetEnvironmentVariable("queuefake.thread.pool.max")) :
            8));

        private IConsume _consumer;

        /// <summary>
        /// 入队消息。
        /// </summary>
        /// <param name="message">要入队的消息。</param>
        public void Enqueue(object message)
        {
            //如果没有设置消费者 (_consumer == null)，则将消息入队
            if (_consumer == null)
            {
                _queue.Enqueue(message);
            }
            else //如果已经设置了消费者，则立即启动一个新任务来处理消息。
            {
                lock (_taskFactory)
                {
                    _taskFactory.StartNew(() => _consumer.Consume(message));
                }
            }
        }

        /// <summary>
        /// 获取消费者。
        /// </summary>
        /// <returns>消费者对象。</returns>
        public IConsume GetConsumer()
        {
            return _consumer;
        }

        /// <summary>
        /// 设置消费者。
        /// </summary>
        /// <param name="consumer">消费者对象。</param>
        public void SetConsumer(IConsume consumer)
        {
            _consumer = consumer;

            lock (_taskFactory)
            {
                while (_queue.TryDequeue(out var message))
                {
                    if (message != null)
                    {
                        _taskFactory.StartNew(() => _consumer.Consume(message));
                    }
                }
            }
        }
    }
    /// <summary>
    /// 自定义的任务调度器，限制了最大并发任务数，确保任务按需执行
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>
        /// 最大并发任务数
        /// </summary>
        private readonly int _maxConcurrencyLevel;
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private readonly List<Task> _runningTasks = new List<Task>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxConcurrencyLevel">最大并发任务数</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxConcurrencyLevel)
        {
            _maxConcurrencyLevel = maxConcurrencyLevel;

            for (int i = 0; i < _maxConcurrencyLevel; i++)
            {
                Task.Run(() => ProcessTasks());
            }
        }
        /// <summary>
        /// 从任务队列中取出任务并执行。
        /// 将任务添加到正在运行的任务列表中，执行后将其移除。
        /// </summary>
        private void ProcessTasks()
        {
            while (true)
            {
                Task task;
                if (_tasks.TryTake(out task))
                {
                    _runningTasks.Add(task);
                    TryExecuteTask(task);
                    _runningTasks.Remove(task);
                }
            }
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }
        /// <summary>
        /// 尝试在当前线程中执行任务，如果任务之前已经排队，则返回 false。
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns></returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued)
            {
                return false;
            }

            TryExecuteTask(task);
            return true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks;
        }

        //protected override int MaximumConcurrencyLevel => _maxConcurrencyLevel;

        protected override bool TryDequeue(Task task)
        {

            return _tasks.TryTake(out task);
        }
    }

}
