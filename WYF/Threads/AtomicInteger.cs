using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public class AtomicInteger
    {
        private volatile int _value;

        public AtomicInteger(int initialValue)
        {
            _value = initialValue;
        }

        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        public int DecrementAndGet()
        {
            return Interlocked.Decrement(ref _value);
        }
    }
}
