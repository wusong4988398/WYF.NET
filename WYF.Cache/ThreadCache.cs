using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache
{
    /// <summary>
    /// 为每个线程提供一个独立的缓存，支持常见的缓存操作（如添加、获取、移除等），并且提供了懒加载机制，
    /// 可以在缓存中找不到数据时自动加载数据并存储到缓存中。这在多线程环境中非常有用，可以提高性能并减少重复计算。
    /// </summary>
    public class ThreadCache : IDisposable
    {
        // 使用AsyncLocal来存储每个线程的缓存实例
        private static readonly AsyncLocal<ThreadCache> _threadLocal = new AsyncLocal<ThreadCache>();

        private ConcurrentDictionary<object, object> _map;

        // 获取当前线程的缓存实例
        public static ThreadCache Current
        {
            get
            {
                if (_threadLocal.Value == null)
                {
                    _threadLocal.Value = new ThreadCache();
                }
                return _threadLocal.Value;
            }
        }

        // 清除当前线程的缓存实例
        public static void Clear()
        {
            _threadLocal.Value = null;
        }

        // 添加键值对到缓存
        public static void Put(object key, object value)
        {
            Current._map[key] = value;
        }

        // 从缓存中获取值
        public static object Get(object key)
        {
            return Current._map.ContainsKey(key) ? Current._map[key] : null;
        }

        // 从缓存中获取值，如果不存在则使用提供的加载器加载并存储
        public static T Get<T>(object key, Func<T> loader)
        {
            return Get(key, loader, true);
        }

        // 从缓存中获取值，如果不存在则使用提供的加载器加载并存储（可选是否存储）
        public static T Get<T>(object key, Func<T> loader, bool putToCache)
        {
            if (Current._map.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            T loadedValue = loader();
            if (loadedValue != null && putToCache)
            {
                Put(key, loadedValue);
            }

            return loadedValue;
        }

        // 从缓存中移除键值对
        public static void Remove(object key)
        {
            Current._map.TryRemove(key, out _);
        }

        // 检查缓存中是否存在指定键
        public static bool Exists(object key)
        {
            return Current._map.ContainsKey(key);
        }

        // 私有构造函数
        private ThreadCache()
        {
            _map = new ConcurrentDictionary<object, object>();
        }

        // 实现IDisposable接口
        public void Dispose()
        {
            _map = null;
        }
    }
}

