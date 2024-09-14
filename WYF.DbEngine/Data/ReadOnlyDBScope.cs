using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine
{
    /// <summary>
    /// 为只读数据库操作提供一个作用域管理机制。它主要用于确保在一个特定的作用域内，数据库操作可以以只读的方式执行，
    /// 并且可以在多线程环境下正确地管理会话计数和资源清理。
    /// </summary>
    public class ReadOnlyDBScope : IDisposable
    {

        private int dbCommandTimeOut;
        private bool disposed;
        [ThreadStatic]
        private static int sessionCount;
        [ThreadStatic]
        private static ReadOnlyDBScope staticReadOnlyDBScope;


        public ReadOnlyDBScope(int dbCommandTimeOut = -1)
        {
            this.dbCommandTimeOut = -1;
            if (Current == null)
            {
                Current = new ReadOnlyDBScope(true, dbCommandTimeOut);
            }
            Interlocked.Increment(ref sessionCount);
        }

        private ReadOnlyDBScope(bool first, int dbCommandTimeOut)
        {
            this.dbCommandTimeOut = -1;
            this.dbCommandTimeOut = dbCommandTimeOut;
        }

        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                long num = Interlocked.Decrement(ref sessionCount);
                if (num == 0L)
                {
                    staticReadOnlyDBScope = null;
                }
                this.disposed = true;
            }
        }


        public int CommandTimeOut
        {
            get
            {
                return this.dbCommandTimeOut;
            }
        }

        public static ReadOnlyDBScope Current
        {
            get
            {
                return staticReadOnlyDBScope;
            }
            private set
            {
                staticReadOnlyDBScope = value;
            }
        }
    }
}
