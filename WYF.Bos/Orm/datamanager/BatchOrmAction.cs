using WYF.Bos.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class BatchOrmAction<DataT, OidT> : IEnumerable<DataT>, IEnumerable
    {
        // Fields
        private IEnumerable<OidT[]> _batchIds;
        private Func<OidT[], DataT[]> _func;
        private bool _notFirstGet;

        // Methods
        public BatchOrmAction(IEnumerable<OidT[]> batchIds, Func<OidT[], DataT[]> func)
        {
            this._batchIds = batchIds;
            this._func = func;
        }

        public IEnumerator<DataT> GetEnumerator()
        {
            if (this._notFirstGet)
            {
                throw new ORMDesignException("002032030001507", "对于大批量数据的操作，不支持对结果的多次枚举(ForEach操作)，因为这样会造成重复的数据库操作，考虑优化您的程序以避免多次枚举");
            }
            this._notFirstGet = true;
            return new BatchOrmEnumerator((BatchOrmAction<DataT, OidT>)this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        private class BatchOrmEnumerator : IEnumerator<DataT>, IDisposable, IEnumerator
        {
 
            private DataT _current;
            private int _currentIndex;
            private DataT[] _currentValues;
            private IEnumerator<OidT[]> _enumerator;
            private bool _moveNextValue;
            private BatchOrmAction<DataT, OidT> batchOrmAction;

       
            public BatchOrmEnumerator(BatchOrmAction<DataT, OidT> batchOrmAction)
            {
                this.batchOrmAction = batchOrmAction;
                this._enumerator = batchOrmAction._batchIds.GetEnumerator();
                this._current = default(DataT);
                this._currentIndex = 0;
                this._moveNextValue = true;
            }



            public void Dispose()
            {
                this._enumerator.Dispose();
            }

            private bool GetNextResult()
            {
                bool flag = this._enumerator.MoveNext();
                if (flag)
                {
                    this._currentValues = this.batchOrmAction._func(this._enumerator.Current);
                    this._currentIndex = 0;
                    return flag;
                }
                this._currentValues = null;
                this._currentIndex = 0;
                this._current = default(DataT);
                return flag;
            }

            public bool MoveNext()
            {
                if (this._moveNextValue)
                {
                    while ((this._currentValues == null) || (this._currentIndex >= this._currentValues.Length))
                    {
                        this._moveNextValue = this.GetNextResult();
                        if (!this._moveNextValue)
                        {
                            return false;
                        }
                        if (this._currentIndex < this._currentValues.Length)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    return false;
                }
                this._current = this._currentValues[this._currentIndex];
                this._currentIndex++;
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

   
            public DataT Current
            {
                get
                {
                    return this._current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this._current;
                }
            }
        }
    }
}
