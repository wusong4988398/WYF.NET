using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.Cache
{
    public interface ILocalMemoryCache
    {
        void Put(string key, object value);

        object? Get(string key);

        IDictionary<string, object> Get(params string[] keys);

        void Remove(params string[] keys);

        void RemoveMapFields(string key, params string[] fields);

        bool Contains(string key);

        void Clear();
    }
}
