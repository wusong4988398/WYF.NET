using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using ZiggyCreatures.Caching.Fusion;
using FreeRedis;


namespace WYF.Cache.redis
{
    public static class RedisFactory
    {
       public static RedisClient _client;


        public static FusionCache GetFusionRedisClient(string redisConnectionString)
        {
           
            var redis = new RedisCache(new RedisCacheOptions() { Configuration = redisConnectionString });

         
            var seroptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            };
            var serializer = new FusionCacheSystemTextJsonSerializer(seroptions);

            // INSTANTIATE FUSION CACHE
            var cache = new FusionCache(new FusionCacheOptions()
            {
                //CacheName = this.region,
                //CacheKeyPrefix = this.region,
                DefaultEntryOptions = new FusionCacheEntryOptions()
                {
                    Duration = TimeSpan.FromMinutes(1), //základní doba trvanlivosti
                    IsFailSafeEnabled = true,
                    FailSafeThrottleDuration = TimeSpan.FromSeconds(30), //základní doba trvanlivosti se posune o minutu, pokud je db down
                    FailSafeMaxDuration = TimeSpan.FromMinutes(3), //maximální doba trvanlivosti - po této době se starý záznam maže
                    FactorySoftTimeout = TimeSpan.FromMilliseconds(150), //kolik času má factory na odpověď při expiraci základní doby trvanlivosti. Jinak se použije expirovaný záznam

                    DistributedCacheDuration = TimeSpan.FromDays(7), //základní doba trvanlivosti v distributed cache
                    DistributedCacheFailSafeMaxDuration = TimeSpan.FromDays(365), //maximální doba trvanlivosti v distributed cache
                    AllowBackgroundDistributedCacheOperations = true,
                    DistributedCacheSoftTimeout = TimeSpan.FromMilliseconds(300)

                }
            });

          
            cache.SetupDistributedCache(redis, serializer);

            return cache;
        }


        public static RedisClient GetRedisClient(string constr)
        {
            if (_client == null)
            {
                _client = new RedisClient(constr);


            }
            return _client;
        }


        //public static RedisCache GetRedisClient(string constr)
        //{
        //    if (_cache==null)
        //    {
        //         _cache = new RedisCache(new RedisCacheOptions()
        //        {
        //            Configuration = constr
        //        });
                

        //    }
        //    return _cache;
        //}
    }
}
