using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public class LocaleValue<T> : IDictionary<string, T>, ISerializable
    {
        private static int langCapacity = 3;
        private string[] keys = new string[langCapacity];
        private T[] values = new T[langCapacity];
        private int size = 0;
        public LocaleValue() { }
        public LocaleValue(string localeId, T value)
        {
            SetItem(localeId, value);
        }

        public LocaleValue(T value)
        {
            
            string localeId = LangExtensions.Get().ToString();
            SetItem(localeId, value);
        }

        public T GetItem(string lcid)
        {
            return this[lcid];
        }

        public void SetItem(string localeId, T value)
        {
            this[localeId] = value;
        }

        public T GetDefaultItem()
        {
            return this[LangExtensions.Get().ToString()];
        }

        public int Count => size;

        public bool IsReadOnly => false;

        public ICollection<string> Keys => keys.Take(size).ToArray();

        public ICollection<T> Values => values.Take(size).ToArray();

        public T this[string key]
        {
            get
            {
                for (int i = 0; i < size; i++)
                {
                    if (keys[i] == null || keys[i].Equals(key))
                        return values[i];
                }
                return default;
            }
            set
            {
                PutVal(key, value);
            }
        }

        private void PutVal(string key, T value)
        {
            for (int i = 0; i < size; i++)
            {
                if (keys[i].Equals(key))
                {
                    values[i] = value;
                    return;
                }
            }
            if (size >= keys.Length)
            {
                Array.Resize(ref keys, size + 1);
                Array.Resize(ref values, size + 1);
            }
            keys[size] = key;
            values[size] = value;
            size++;
        }

        public void Add(string key, T value)
        {
            PutVal(key, value);
        }

        public bool ContainsKey(string key)
        {
            return keys.Take(size).Contains(key);
        }

        public bool Remove(string key)
        {
            for (int i = 0; i < size; i++)
            {
                if (keys[i] != null && keys[i].Equals(key))
                {
                    T value = values[i];
                    values = RemoveByIndex(values, i);
                    keys = RemoveByIndex(keys, i);
                    size--;
                    return true;
                }
            }
            return false;
        }

        private A[] RemoveByIndex<A>(A[] oldArr, int index)
        {
            List<A> list = new List<A>(oldArr);
            list.RemoveAt(index);
            if (list.Count < langCapacity)
                list.Add(default);
            return list.ToArray();
        }

        public bool TryGetValue(string key, out T value)
        {
            value = this[key];
            return value != null;
        }

        public void Add(KeyValuePair<string, T> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Array.Clear(keys, 0, size);
            Array.Clear(values, 0, size);
            size = 0;
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return ContainsKey(item.Key) && EqualityComparer<T>.Default.Equals(this[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            for (int i = 0; i < size; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<string, T>(keys[i], values[i]);
            }
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            for (int i = 0; i < size; i++)
            {
                yield return new KeyValuePair<string, T>(keys[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("keys", keys);
            info.AddValue("values", values);
            info.AddValue("size", size);
        }

        public LocaleValue(SerializationInfo info, StreamingContext context)
        {
            keys = (string[])info.GetValue("keys", typeof(string[]));
            values = (T[])info.GetValue("values", typeof(T[]));
            size = info.GetInt32("size");
        }
    }
}
