using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WYF.Mq.broadcast
{
    public class BroadcastService
    {

        public static void Start()
        {
            try
            {
                Broadcast.GetBroadcast().RegisterBroadcastConsumer();

            }
            catch (Exception e)
            {
                //logger.Error(e);
            }
        }

        public static void BroadcastMessage(string className, string methodName, object[] parameters)
        {
            BroadcastMessageInternal(className, methodName, parameters);
        }

        public static void BroadcastMessageWithApp(string appId, string className, string methodName, object[] parameters)
        {
            BroadcastMessageWithAppInternal(appId, className, methodName, parameters);
        }

        private static void BroadcastMessageInternal(string className, string methodName, object[] parameters)
        {
            var item = new BroadcastItem
            {
                ClassName = className,
                MethodName = methodName,
                Params = parameters
            };

            byte[] messageBytes = ToByteArray(item);
            Broadcast.GetBroadcast().BroadcastMessage(messageBytes);
        }

        private static void BroadcastMessageWithAppInternal(string appId, string className, string methodName, object[] parameters)
        {
            var item = new BroadcastItem
            {
                ClassName = className,
                MethodName = methodName,
                Params = parameters
            };

            byte[] messageBytes = ToByteArray(item);
            Broadcast.GetBroadcast().BroadcastMessage(appId, messageBytes);
        }

        //private static byte[] ToByteArray(object obj)
        //{
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        var binaryFormatter = new BinaryFormatter();
        //        binaryFormatter.Serialize(memoryStream, obj);
        //        return memoryStream.ToArray();
        //    }
        //}


        public static byte[] ToByteArray<T>(T obj)
        {
            // 使用System.Text.Json将对象序列化为JSON字符串
            string jsonString = JsonSerializer.Serialize(obj);

            // 将JSON字符串转换为字节数组
            return System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
