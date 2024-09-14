using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WYF.Common;

namespace WYF.DbEngine
{
    /// <summary>
    /// 提供了一种机制来管理数据库连接的作用域，确保每个会话都有一个独立的连接，并且在会话结束时正确释放资源。
    /// 这对于需要在多线程环境中管理数据库连接的应用程序特别有用，可以避免资源泄漏和其他潜在的问题。
    /// 通过使用 Interlocked 操作和 ThreadLocal（如果使用现代替代方案的话），它确保了线程安全性和资源的正确管理
    /// </summary>
    public class SessionScope : IDisposable, ISessionScope
    {
        /// <summary>
        /// 存储当前会话的数据库连接
        /// </summary>
        public DbConnection Connection;
        public string connectionString;
        /// <summary>
        /// 标记对象是否已经释放
        /// </summary>
        private bool disposed;
        /// <summary>
        /// 记录当前活跃会话的数量
        /// </summary>
        [ThreadStatic]
        private static int sessionCount;
        /// <summary>
        /// 记录当前活跃的 SessionScope 实例
        /// </summary>
        [ThreadStatic]
        private static SessionScope staticSessionScope;
        public SessionScope()
        {
            if (Current == null)
            {
                Current = new SessionScope(true);
            }
            Interlocked.Increment(ref sessionCount);
        }

        private SessionScope(bool first)
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                long num = Interlocked.Decrement(ref sessionCount);
                if (num == 0L)
                {
                    if (staticSessionScope.Connection != null)
                    {
                        if (Transaction.Current == null)
                        {
                            staticSessionScope.Connection.Dispose();
                        }
                        staticSessionScope.Connection = null;
                    }
                    staticSessionScope = null;
                }
                this.disposed = true;
            }
        }
        /// <summary>
        /// 调用 Dispose(false)，确保在对象被垃圾回收之前释放资源。
        /// </summary>
        ~SessionScope()
        {
            this.Dispose(false);
        }

 
        public static SessionScope Current
        {
            get
            {
                return staticSessionScope;
            }
            private set
            {
                staticSessionScope = value;
            }
        }
    }
}
