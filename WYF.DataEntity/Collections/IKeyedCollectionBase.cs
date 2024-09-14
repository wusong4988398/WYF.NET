using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Collections
{
    public interface IKeyedCollectionBase<TKey, TValue> : IEnumerable<TValue>, IEnumerable
    {
        // Token: 0x06000160 RID: 352
        bool Contains(TValue item);

        // Token: 0x06000161 RID: 353
        bool ContainsKey(TKey key);

        // Token: 0x06000162 RID: 354
        int IndexOf(TValue item);

        // Token: 0x06000163 RID: 355
        void CopyTo(TValue[] array, int arrayIndex);

        // Token: 0x06000164 RID: 356
        TValue[] ToArray();

        // Token: 0x17000049 RID: 73
        TValue this[int index]
        {
            get;
        }

        // Token: 0x1700004A RID: 74
        TValue this[TKey key]
        {
            get;
        }

        // Token: 0x06000167 RID: 359
        bool TryGetValue(TKey key, out TValue value);

        // Token: 0x1700004B RID: 75
        // (get) Token: 0x06000168 RID: 360
        int Count { get; }
    }
}
