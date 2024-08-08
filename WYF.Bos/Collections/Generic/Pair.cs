using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Collections.Generic
{
    [Serializable]
    public sealed class Pair<T>
    {

        public T[] Array;
        public Pair<T> Previous;
    }
}
