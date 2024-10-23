using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.dataManager
{
    internal sealed class ObjectCache<TKey, TValue> where TKey : class where TValue : class
    {

        private readonly ConditionalWeakTable<TKey, TValue> _cache;


        private ObjectCache()
        {
            this._cache = new ConditionalWeakTable<TKey, TValue>();
        }

        public static ObjectCache<TKey, TValue> Create()
        {
            return new ObjectCache<TKey, TValue>();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return this._cache.GetValue(key, k => valueFactory(k));
        }
    }
}
