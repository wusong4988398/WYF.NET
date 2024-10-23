using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache
{
    public class CacheFactoryBuilder
    {
        private static readonly CacheFactoryBuilder defaultBuilder = new CacheFactoryBuilder();
        private static volatile CacheFactory instance = null;
        private static readonly object syncLock = new object();


        // 获取默认的构建器
        public static CacheFactoryBuilder NewBuilder()
        {
            return defaultBuilder;
        }

        // 构建缓存工厂实例
        public CacheFactory Build()
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new CacheFactoryImpl(); 
                    }
                }
            }
            return instance;
        }
    }
}
