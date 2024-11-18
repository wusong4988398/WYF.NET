using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Mq.broadcast;

namespace WYF.Fake.mq.rabbitmqfake
{
    public class RabbitBroadcastConsumerFake : IConsume
    {
        public void Consume(object message)
        {
            MessageReceive.Instance.OnMessage((byte[])message);
        }
    }
}
