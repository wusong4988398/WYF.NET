using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Utils
{
    [Serializable]
    public class StringIntMap : KeyIntMap
    {
        private static readonly Entry NULL_ENTRY = new Entry(null, 0, -1, null);
        private StringIntMap _readOnlyMap;
        private Entry _lastEntry;
        private Entry[] _table;
        private int _index;
        private int _totalCharacterCount;

        public StringIntMap(int initialCapacity, float loadFactor) : base(initialCapacity, loadFactor)
        {
            _lastEntry = NULL_ENTRY;
            _table = new Entry[_capacity];
        }

        public StringIntMap(int initialCapacity) : this(initialCapacity, DEFAULT_LOAD_FACTOR) { }

        public StringIntMap() : this(DEFAULT_INITIAL_CAPACITY, DEFAULT_LOAD_FACTOR) { }

        public override void Clear()
        {
            for (int i = 0; i < _table.Length; i++)
                _table[i] = null;
            _lastEntry = NULL_ENTRY;
            _size = 0;
            _index = _readOnlyMapSize;
            _totalCharacterCount = 0;
        }

        public override void SetReadOnlyMap(KeyIntMap readOnlyMap, bool clear)
        {
            if (!(readOnlyMap is StringIntMap))
                throw new ArgumentException("illegalClass");
            SetReadOnlyMap((StringIntMap)readOnlyMap, clear);
        }

        public void SetReadOnlyMap(StringIntMap readOnlyMap, bool clear)
        {
            _readOnlyMap = readOnlyMap;
            if (_readOnlyMap != null)
            {
                _readOnlyMapSize = _readOnlyMap.Size;
                _index = _size + _readOnlyMapSize;
                if (clear)
                    Clear();
            }
            else
            {
                _readOnlyMapSize = 0;
                _index = _size;
            }
        }

        public int GetNextIndex()
        {
            return _index++;
        }

        public int GetIndex()
        {
            return _index;
        }

        public void Put(string key, int value)
        {
            int hash = HashHash(key.GetHashCode());
            int tableIndex = IndexFor(hash, _table.Length);
            AddEntry(key, value, hash, tableIndex);
        }

        public int Get(string key)
        {
            return (key == _lastEntry._key) ? _lastEntry._value : Get(key, HashHash(key.GetHashCode()));
        }

        public int GetTotalCharacterCount()
        {
            return _totalCharacterCount;
        }

        private int Get(string key, int hash)
        {
            int tableIndex = IndexFor(hash, _table.Length);
            for (Entry e = _table[tableIndex]; e != null; e = e._next)
            {
                if (e._hash == hash && Eq(key, e._key))
                {
                    _lastEntry = e;
                    return e._value;
                }
            }
            return NOT_PRESENT;
        }

        private void AddEntry(string key, int value, int hash, int bucketIndex)
        {
            Entry e = _table[bucketIndex];
            _table[bucketIndex] = new Entry(key, hash, value, e);
            _totalCharacterCount += key.Length;
            if (_size++ >= _threshold)
                Resize(2 * _table.Length);
        }

        protected void Resize(int newCapacity)
        {
            _capacity = newCapacity;
            Entry[] oldTable = _table;
            int oldCapacity = oldTable.Length;
            if (oldCapacity == MAXIMUM_CAPACITY)
            {
                _threshold = int.MaxValue;
            }
            else
            {
                Entry[] newTable = new Entry[_capacity];
                Transfer(newTable);
                _table = newTable;
                _threshold = (int)(_capacity * _loadFactor);
            }
        }

        private void Transfer(Entry[] newTable)
        {
            Entry[] src = _table;
            int newCapacity = newTable.Length;
            for (int j = 0; j < src.Length; j++)
            {
                Entry e = src[j];
                if (e != null)
                {
                    Entry next;
                    src[j] = null;
                    do
                    {
                        next = e._next;
                        int i = IndexFor(e._hash, newCapacity);
                        e._next = newTable[i];
                        newTable[i] = e;
                        e = next;
                    } while (next != null);
                }
            }
        }

        private bool Eq(string x, string y)
        {
            return x == y || x.Equals(y);
        }

        [Serializable]
        protected class Entry : BaseEntry
        {
            public readonly string _key;
            public Entry _next;

            public Entry(string key, int hash, int value, Entry next) : base(hash, value)
            {
                _key = key;
                _next = next;
            }
        }
    }
}
