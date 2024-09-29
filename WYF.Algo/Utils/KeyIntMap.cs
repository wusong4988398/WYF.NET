using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Utils
{
    [Serializable]
    public abstract class KeyIntMap
    {
        public const int NOT_PRESENT = -1;
        protected const int DEFAULT_INITIAL_CAPACITY = 16;
        protected const int MAXIMUM_CAPACITY = 1048576;
        protected const float DEFAULT_LOAD_FACTOR = 0.75f;

        protected int _readOnlyMapSize;
        protected int _size;
        protected int _capacity;
        protected int _threshold;
        protected readonly float _loadFactor;

        protected KeyIntMap(int initialCapacity, float loadFactor)
        {
            if (initialCapacity < 0)
                throw new ArgumentException("illegal initialCapacity " + initialCapacity);
            if (initialCapacity > MAXIMUM_CAPACITY)
                initialCapacity = MAXIMUM_CAPACITY;
            if (loadFactor > 0.0f && !float.IsNaN(loadFactor))
            {
                if (initialCapacity != DEFAULT_INITIAL_CAPACITY)
                {
                    _capacity = 1;
                    while (_capacity < initialCapacity)
                        _capacity <<= 1;
                    _loadFactor = loadFactor;
                    _threshold = (int)(_capacity * _loadFactor);
                }
                else
                {
                    _capacity = DEFAULT_INITIAL_CAPACITY;
                    _loadFactor = DEFAULT_LOAD_FACTOR;
                    _threshold = 12;
                }
            }
            else
            {
                throw new ArgumentException("illegal LoadFactor " + loadFactor);
            }
        }

        protected KeyIntMap(int initialCapacity) : this(initialCapacity, DEFAULT_LOAD_FACTOR) { }

        protected KeyIntMap()
        {
            _capacity = DEFAULT_INITIAL_CAPACITY;
            _loadFactor = DEFAULT_LOAD_FACTOR;
            _threshold = 12;
        }

        public int Size => _size + _readOnlyMapSize;

        public abstract void Clear();

        public abstract void SetReadOnlyMap(KeyIntMap readOnlyMap, bool clear);

        public static int HashHash(int h)
        {
            h += (h << 9) ^ ~0;
            h ^= (h >> 14);
            h += (h << 4);
            h ^= (h >> 10);
            return h;
        }

        public static int IndexFor(int h, int length)
        {
            return h & (length - 1);
        }

        [Serializable]
        public class BaseEntry
        {
            public readonly int _hash;
            public readonly int _value;

            public BaseEntry(int hash, int value)
            {
                _hash = hash;
                _value = value;
            }
        }
    }
}
