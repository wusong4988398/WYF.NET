using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class ArrayBatchEnumerable : IEnumerable<object[]>, IEnumerable
    {

        private readonly int _batchSize;
        private readonly object[] _idsArray;


        public ArrayBatchEnumerable(object[] idsArray, int batchSize)
        {
            this._idsArray = idsArray;
            this._batchSize = batchSize;
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return new ArrayBatchEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct ArrayBatchEnumerator : IEnumerator<object[]>, IDisposable, IEnumerator
        {
            private ArrayBatchEnumerable _owner;
            private int _pageIndex;
            private object[] _current;
            public ArrayBatchEnumerator(ArrayBatchEnumerable arrayBatchEnumerable)
            {
                this._owner = arrayBatchEnumerable;
                this._pageIndex = 0;
                this._current = null;
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
                int sourceIndex = this._pageIndex * this._owner._batchSize;
                int num2 = (this._pageIndex + 1) * this._owner._batchSize;
                if (sourceIndex >= this._owner._idsArray.Length)
                {
                    return false;
                }
                if (num2 >= this._owner._idsArray.Length)
                {
                    num2 = this._owner._idsArray.Length;
                }
                int length = num2 - sourceIndex;
                object[] destinationArray = new object[length];
                Array.Copy(this._owner._idsArray, sourceIndex, destinationArray, 0, length);
                this._current = destinationArray;
                this._pageIndex++;
                return true;
            }

            public void Reset()
            {
                this._pageIndex = 0;
                this._current = null;
            }
        }
    }
}
