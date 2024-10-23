using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace WYF.Cache.redis
{
    public class RedisSessionlessCache : AbstractRedisSessionCache, IDistributeSessionlessCache
    {
        private string region;
        public RedisSessionlessCache(string region)
        {
           this.region = ((region == null) ? "" : region);
        }

   



        public int AddList(string key, params string[] values)
        {
            throw new NotImplementedException();
        }

        public  void Put(string key, string value)
        {
            this.Put(key,value,TimeSpan.FromMinutes(60));
        }


        public void Put(string key, string value, TimeSpan timeout)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            client.Set<string>(key, value,timeout);


        }


        public string GetRedisUrl()
        {
            return "127.0.0.1:6379,password=Hairuan@123";
        }

        public List<string> Get(string type, params string[] keys)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            List<string> result= client.HMGet(type, keys).ToList();
            return result;
        }

        public void Put(string type, Dictionary<string, string> keyValues)
        {
            this.Put(type, keyValues, this.GetDefaultTimeout());
        }

        private TimeSpan GetDefaultTimeout()
        {
           return TimeSpan.FromMinutes(30);
        }

        public void Put(string type, Dictionary<string, string> keyValues, TimeSpan timeout)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            client.HMSet(type, keyValues);
            client.Expire(type, timeout.Seconds);


        }

        public void Remove(params string[] key)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            client.Del(key);
        }

   

   

        public void Remove(string type, params string[] keys)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            client.HDel(type, keys);

        }

        public void Remove(string type, string key)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetAll(string type)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            Dictionary<string, string> result= client.HGetAll(type);
            return result;
        }

        public string Get(string type, string key)
        {
            RedisClient client = RedisFactory.GetRedisClient(GetRedisUrl());
            return client.HGet(type, key);
        }
    }
}
