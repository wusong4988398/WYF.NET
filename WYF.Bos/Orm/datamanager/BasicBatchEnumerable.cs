using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class BasicBatchEnumerable : IEnumerable<object[]>, IEnumerable
    {
        private readonly int _batchSize;
        private readonly IEnumerable _col;
        public BasicBatchEnumerable(IEnumerable col, int batchSize)
        {
            this._col = col;
            this._batchSize = batchSize;
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return new BasicBatchEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private struct BasicBatchEnumerator : IEnumerator<object[]>, IDisposable, IEnumerator
        {
            private readonly BasicBatchEnumerable _owner;
            private object[] _current;
            private IEnumerator _enumerator;
            private bool _moveNextResult;
            public BasicBatchEnumerator(BasicBatchEnumerable basicBatchEnumerable)
            {
                this._owner = basicBatchEnumerable;
                this._enumerator = this._owner._col.GetEnumerator();
                this._current = null;
                this._moveNextResult = true;
            }

            public object[] Current
            {
                get
                {
                    return this._current;
                }
            }
            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return this._current;
                }
            }
            public bool MoveNext()
            {
                if (!this._moveNextResult)
                {
                    return false;
                }
                object[] sourceArray = new object[this._owner._batchSize];
                int index = 0;
                do
                {
                    this._moveNextResult = this._enumerator.MoveNext();
                    if (!this._moveNextResult)
                    {
                        int length = index;
                        this._current = new object[length];
                        Array.Copy(sourceArray, 0, this._current, 0, length);
                        return true;
                    }
                    sourceArray[index] = this._enumerator.Current;
                    index++;
                }
                while (index != this._owner._batchSize);
                this._current = sourceArray;
                return true;
            }

            public void Reset()
            {
                this._moveNextResult = true;
                this._current = null;
                this._enumerator.Reset();
            }
        }
    }
}
