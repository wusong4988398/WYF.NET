using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WYF.Algo.Utils
{
    [Serializable]
    public abstract class Resource : IDisposable
    {
        private  bool closed;
        private int refCount = 0;

        public void Refer()
        {
            Interlocked.Increment(ref refCount);
        }

        public void Close()
        {
            if (closed)
                return;
            if (Unrefer())
                closed = true;
        }

        public bool IsClosed()
        {
            return closed;
        }

        public bool Unrefer()
        {
            int i = Interlocked.Decrement(ref refCount);
            if (i < 0)
                i = 0;
            if (i == 0)
            {
                RealClose();
                return true;
            }
            return false;
        }

        public int GetReferCount()
        {
            return refCount;
        }

        public abstract void CheckClosed();

        public abstract void RealClose();

        public void Dispose()
        {
            Close();
        }
    }
}
