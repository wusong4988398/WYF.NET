
using System.Collections;
using System.Runtime.Serialization;
namespace WYF.DataEntity.Collections

{
    [Serializable]
    public abstract class KeyedCollectionBase<TKey, TValue> : List<TValue>, IKeyedCollectionBase<TKey, TValue>, IEnumerable<TValue>, IEnumerable, IDeserializationCallback
    {

        public KeyedCollectionBase()
        {
            this.CreateDictionary();
        }
        protected KeyedCollectionBase(IList<TValue> list) : base(list)
        {
            this._comparer = EqualityComparer<TKey>.Default;
            this.CreateDictionary();
        }
        public  new   void Add(TValue element)
        {
            //this._dict.put(GetKeyForItem(element), element);

            this._dict[GetKeyForItem(element)] = element;
            base.Add(element);
            //base.Items.Add(element);
            
        }

        private void CreateDictionary()
        {
            this._dict = new Dictionary<TKey, TValue>(this.Comparer);
            foreach (TValue tvalue in this)
            {
                TKey keyForItem = this.GetKeyForItem(tvalue);
                if (keyForItem != null)
                {
                    this._dict.Add(keyForItem, tvalue);
                }
            }
        }


        protected IDictionary<TKey, TValue> Dictionary
        {
            get
            {
                return this._dict;
            }
        }

        protected virtual IEqualityComparer<TKey> Comparer
        {
            get
            {
                return this._comparer;
            }
        }


        protected abstract TKey GetKeyForItem(TValue item);


        public bool ContainsKey(TKey key)
        {
            TValue tvalue;
            return this.TryGetValue(key, out tvalue);
        }


        public TValue[] ToArray()
        {
            TValue[] array = new TValue[base.Count];
            base.CopyTo(array, 0);
            return array;
        }


        public TValue this[TKey key]
        {
            get
            {
                TValue result;
                if (this.TryGetValue(key, out result))
                {
                    return result;
                }
                return result;
                //throw new KeyNotFoundException();
            }
        }


        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dict.TryGetValue(key, out value);
        }


        public virtual void OnDeserialization(object sender)
        {
            this._comparer = EqualityComparer<TKey>.Default;
            this.CreateDictionary();
        }

   

        //TValue IKeyedCollectionBase<TKey, TValue>.get_Item(int A_1)
        //{

        //    return base[A_1];
        //}


        [NonSerialized]
        private IEqualityComparer<TKey> _comparer;


        [NonSerialized]
        private Dictionary<TKey, TValue> _dict;
    }
}
