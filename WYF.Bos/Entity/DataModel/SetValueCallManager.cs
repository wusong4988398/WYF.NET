using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    internal sealed class SetValueCallManager<T> : IEnumerable<T>, IEnumerable
    {

        private int _callCount;
        private Stack<T> _callStack;
        private T _firstCall;


        public SetValueCallManager()
        {
            this._callStack = new Stack<T>(4);
        }

        public bool Contains(T callInfo)
        {
            if (this._callCount > 0)
            {
                if (object.Equals(callInfo, this._firstCall))
                {
                    return true;
                }
                if (this._callCount > 1)
                {
                    foreach (T local in this._callStack)
                    {
                        if (object.Equals(local, callInfo))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if ((this._callCount != 0) && (this._callCount > 0))
            {
                yield return this._firstCall;
                if (this._callCount > 1)
                {
                    foreach (T iteratorVariable0 in this._callStack)
                    {
                        yield return iteratorVariable0;
                    }
                }
            }
        }

        public void Pop()
        {
            if (this._callCount == 0)
            {
                throw new InvalidOperationException("调用错误，当前没有任何压栈，不能出栈");
            }
            if (this._callCount == 1)
            {
                this._firstCall = default(T);
            }
            else
            {
                this._callStack.Pop();
            }
            this._callCount--;
        }

        public bool Push(T callInfo)
        {
            if ((this._callCount > 0) && this.Contains(callInfo))
            {
                return false;
            }
            if (this._callCount == 0)
            {
                this._firstCall = callInfo;
            }
            else
            {
                this._callStack.Push(callInfo);
            }
            this._callCount++;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(" >> ", (from p in this select p.ToString()).ToArray<string>());
        }


    }
}
