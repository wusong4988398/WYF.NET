using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache
{
    public interface ISessionlessCache<T>
    {
        void Put(string key, T value);
        void Put(string type, Dictionary<string, T> keyValues);
        void Put(string key, T value, TimeSpan timeout);
        void Put(string type, Dictionary<string, string> keyValues,  TimeSpan timeout);

        void Remove(params string[] keys);
        void Remove(string type, string key);
        void Remove(string type, string[] keys);
        List<T> Get(string type, params string[] keys);
        T Get(string type, string key);

        Dictionary<string, T> GetAll(string type);
    }
}
