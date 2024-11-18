using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using WYF.Common;
using WYF.Fake.mq.rabbitmqfake;
using WYF.Mq.broadcast;

namespace WYF.Fake.mq
{
    public class RabbitBroadcastFake : Broadcast
    {
        protected static readonly string ExchangeName = "exchange_fanout_" + Instance.GetClusterName();
        private readonly IConsume _consumer = new RabbitBroadcastConsumerFake();

        public override void RegisterBroadcastConsumer()
        {
            try
            {
                Channel.QueueDeclare(ExchangeName, null);
                Channel.BasicConsume(ExchangeName, null, _consumer);
            }
            catch (Exception e)
            {
                throw new Exception("can't init channel",e);
            }
        }

        public override void BroadcastMessage(byte[] message)
        {
            try
            {
                Channel.Publish(ExchangeName, null, message);
            }
            catch (Exception e)
            {
                throw new Exception("broadcast message error",e);
            }
        }

        public override void BroadcastMessage(string appid, byte[] message)
        {
            try
            {
                Channel.Publish(ExchangeName, null, message);
            }
            catch (Exception e)
            {
                throw new Exception("broadcast message error",e);
            }
        }
    }
}
