using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache.Local
{
    public class EhcacheMemoryCacheClusterNotify
    {
        private static Type broadcastType;
        private static MethodInfo broadcastMessage;
        private static CacheFactory factory;
        //private static ILogger logger = LoggerFactory.GetLogger(typeof(EhcacheMemoryCacheClusterNotify));

        static EhcacheMemoryCacheClusterNotify()
        {
            try
            {
                
                broadcastType = Type.GetType("WYF.Mq.broadcast.BroadcastService");
                if (broadcastType != null)
                {
                    foreach (var method in broadcastType.GetMethods())
                    {
                        if (method.Name == "BroadcastMessage")
                        {
                            broadcastMessage = method;
                            break;
                        }
                    }
                }
                factory = CacheFactory.GetCommonCacheFactory();
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "初始化失败");
            }
        }

        private static ThreadLocal<bool> needNotify = new ThreadLocal<bool>(() => true);

        public static void Clear(string region, string type)
        {
            try
            {
                if (NeedLog())
                    //logger.LogInformation($"接收广播清除消息: region={region}, type={type}");
                needNotify.Value = false;
                
                var defaultCacheRegion = factory.GetLocalMemoryCache(region, type);
                if (defaultCacheRegion != null)
                    defaultCacheRegion.Clear();
            }
            finally
            {
                needNotify.Value = true;
            }
        }

        public static void Remove(string region, string type, string[] keys)
        {
            try
            {
                if (NeedLog())
                    //logger.LogInformation($"接收广播移除消息: region={region}, type={type}, keys={string.Join(", ", keys)}");
                needNotify.Value = false;
                var defaultCacheRegion = factory.GetLocalMemoryCache(region, type);
                if (defaultCacheRegion != null)
                    defaultCacheRegion.Remove(keys);
            }
            finally
            {
                needNotify.Value = true;
            }
        }

        public static void RemoveMapFields(string region, string type, string key, string[] fields)
        {
            try
            {
                if (NeedLog())
                    //logger.LogInformation($"接收广播移除字段消息: region={region}, type={type}, key={key}, fields={string.Join(", ", fields)}");
                needNotify.Value = false;
                var defaultCacheRegion = factory.GetLocalMemoryCache(region, type);
                if (defaultCacheRegion != null)
                    defaultCacheRegion.RemoveMapFields(key, fields);
            }
            finally
            {
                needNotify.Value = true;
            }
        }

        public static void NotifySync(string method, params object[] parameters)
        {
            if (needNotify.Value)
                Notify(method, parameters);
        }

        private static void Notify(string method, object[] parameters)
        {
            if (broadcastMessage != null)
            {
                try
                {
                    if (NeedLog())
                        //logger.LogInformation($"发送广播: [{method}] {JsonConvert.SerializeObject(parameters)}");
                    broadcastMessage.Invoke(null, new object[] { typeof(EhcacheMemoryCacheClusterNotify).FullName, method, parameters });
                }
                catch (Exception ex)
                {
                    throw new Exception("广播消息错误");
                }
            }
        }

        private static bool NeedLog()
        {
            return bool.Parse(Environment.GetEnvironmentVariable("BROADCAST_LOG_ENABLE") ?? "false");
        }
    }
}
