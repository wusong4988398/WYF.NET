using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WYF.Common;

namespace WYF.Mq.broadcast
{
    public class MessageReceive
    {
        private readonly ConcurrentDictionary<string, MethodInfo> MethodCache = new ConcurrentDictionary<string, MethodInfo>();
        public static readonly MessageReceive Instance = new MessageReceive();




        public void OnMessage(byte[] messages)
        {
            BroadcastItem item = FromByte<BroadcastItem>(messages);
            DealMessage(item);
        }

        private void DealMessage(BroadcastItem message)
        {
            if (Common.Instance.GetInstanceId() == message.InstanceId)
                return;
            try
            {
                MethodInfo method = null;
                string methodKey = message.ClassName + message.MethodName;
                Type c = Type.GetType(message.ClassName);
                if (!MethodCache.ContainsKey(methodKey))
                {
                    lock (typeof(MessageReceive))
                    {
                        if (!MethodCache.ContainsKey(methodKey))
                        {
                            MethodInfo[] methods = c.GetMethods();
                            foreach (MethodInfo m in methods)
                            {
                                if (m.Name == message.MethodName)
                                {
                                    method = m;
                                    MethodCache.TryAdd(methodKey, method);
                                    break;
                                }
                            }
                        }
                    }
                }
                method = MethodCache[methodKey];
                object instance = Activator.CreateInstance(c);
                method.Invoke(instance, message.Params);
            }
            catch (Exception e)
            {
                //ExceptionLogger.Log("consume broadcast message error", e);
            }
        }

        public static T FromByte<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return JsonSerializer.Deserialize<T>(ms);
            }
        }
    }

}
