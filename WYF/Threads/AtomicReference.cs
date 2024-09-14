using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public class AtomicReference<T> where T:class
    {
        private T _value;

        public AtomicReference(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set => Interlocked.Exchange(ref _value, value);
        }
    }
}
