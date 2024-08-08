using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.database
{
    public class DbMetadataCollection<T> : Collection<T> where T : DbMetadataBase
    {
        private int _lastIndex;

        public DbMetadataCollection()
        {
            this._lastIndex = -1;
        }
        public T this[string name]
        {
            get
            {
                T local;
                if (!this.TryGetValue(name, out local))
                {
                    throw new KeyNotFoundException();
                }
                return local;
            }
        }

        public bool TryGetValue(string name, out T value)
        {
            if (name != null)
            {
                name = name.ToLower();
                if ((this._lastIndex >= 0) && (this._lastIndex < base.Count))
                {
                    value = base[this._lastIndex];
                    if (value.Name.ToLower() == name)
                    {
                        return true;
                    }
                }
                for (int i = 0; i < base.Count; i++)
                {
                    value = base[i];
                    if (value.Name.ToLower() == name)
                    {
                        this._lastIndex = i;
                        return true;
                    }
                }
            }
            value = default(T);
            return false;
        }
    }
}
