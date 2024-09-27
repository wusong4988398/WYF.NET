using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public interface ILocaleValue<T> : IDictionary<string, T>
    {
        T GetItem(string key);
        T GetDefaultItem();
        void SetItem(string key, T value);
    }
}
