using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using WYF.Common;

namespace WYF.Mq.broadcast
{
    public abstract class Broadcast
    {
        //private static readonly ExtensionFactory<Broadcast> BroadcastFactory = ExtensionFactory<Broadcast>.GetFactory(typeof(Broadcast));
        private static volatile Broadcast _broad;
        private static readonly object LockObject = new object();

        public static Broadcast GetBroadcast()
        {
            if (_broad == null)
            {
                lock (LockObject)
                {
                    if (_broad == null)
                    {
                        InitBroad();
                    }
                }
            }
            return _broad;
        }

        private static void InitBroad()
        {
            if (Instance.IsLightWeightDeploy())
            {
                try
                {
                    _broad = (Broadcast)Assembly.Load("WYF.Fake").CreateInstance("WYF.Fake.mq.RabbitBroadcastFake");
                }
                catch (Exception e)
                {
                    throw new Exception("初始化 RabbitFactoryFake 错误:"+e.Message);
                }
            }
            else
            {
                throw new NotImplementedException();
                //string type = QueueManager.GetMQType("");
                //_broad = BroadcastFactory.GetExtension(type);
            }
        }

        public abstract void RegisterBroadcastConsumer();

        public abstract void BroadcastMessage(byte[] message);

        public abstract void BroadcastMessage(string topic, byte[] message);
    }
}
