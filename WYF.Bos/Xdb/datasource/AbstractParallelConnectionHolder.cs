using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;
namespace WYF.Bos.xdb.datasource
{
    /// <summary>
    /// 表示用于管理并行数据库连接的抽象类。
    /// </summary>
    public abstract class AbstractParallelConnectionHolder : IParallelConnectionHolder
    {
        /// <summary>
        /// 同步锁对象。
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 主数据库连接。
        /// </summary>
        private IDbConnection _mainCon;

        /// <summary>
        /// 原子引用，用于追踪正在使用主连接的线程。
        /// </summary>
        private readonly AtomicReference<Thread> _busyMainCon = new AtomicReference<Thread>(null);

        /// <summary>
        /// 空闲查询连接队列。
        /// </summary>
        private readonly ConcurrentQueue<IDbConnection> _freeQueryConList = new ConcurrentQueue<IDbConnection>();

        /// <summary>
        /// 忙碌查询连接队列。
        /// </summary>
        private readonly ConcurrentQueue<IDbConnection> _busyQueryConList = new ConcurrentQueue<IDbConnection>();

        /// <summary>
        /// 字典，用于映射连接到它们的引用计数。
        /// </summary>
        private readonly ConcurrentDictionary<IDbConnection, AtomicInteger> _queryConRefMap = new ConcurrentDictionary<IDbConnection, AtomicInteger>();

        /// <summary>
        /// 初始化 <see cref="AbstractParallelConnectionHolder"/> 类的新实例。
        /// </summary>
        public AbstractParallelConnectionHolder()
        {
            try
            {
                _mainCon = CreateConnection(true, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 需要一个数据库连接以供并行操作使用。
        /// </summary>
        /// <param name="forManager">指示是否为管理器操作需要连接。</param>
        /// <param name="canUseNoneMainConnection">指示是否可以使用非主连接。</param>
        /// <param name="querySQL">将使用连接执行的 SQL 查询。</param>
        /// <returns>数据库连接。</returns>
        public IDbConnection RequireConnection(bool forManager, bool canUseNoneMainConnection, string querySQL)
        {
            if (forManager || !canUseNoneMainConnection)
            {
                return WaitForMainConnection();
            }

            lock (_busyMainCon)
            {
                var currentThread = Thread.CurrentThread;
                var thread = _busyMainCon.Value;
                if (thread == null)
                {
                    _busyMainCon.Value = currentThread;
                    return _mainCon;
                }
                if (thread == currentThread)
                {
                    return _mainCon;
                }
            }
            IDbConnection con = null;
            lock (_lock)
            {
                if (!_freeQueryConList.IsEmpty)
                {
                  
                    if (_freeQueryConList.TryDequeue(out con))
                    {
                        _busyQueryConList.Enqueue(con);
                        _queryConRefMap.GetOrAdd(con, new AtomicInteger(1)).IncrementAndGet();
                        return con;
                    }
                }

                con = CreateConnection(false, querySQL);
                if (con != _mainCon)
                {
                    _busyQueryConList.Enqueue(con);
                    _queryConRefMap.GetOrAdd(con, new AtomicInteger(1));
                    return con;
                }
            }

            return WaitForMainConnection();
        }

        /// <summary>
        /// 等待主连接变得可用。
        /// </summary>
        /// <returns>主数据库连接。</returns>
        private IDbConnection WaitForMainConnection()
        {
            var currentThread = Thread.CurrentThread;
            lock (_busyMainCon)
            {
                try
                {
                    while (_busyMainCon.Value != null && _busyMainCon.Value != currentThread)
                    {
                        Monitor.Wait(_busyMainCon);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                _busyMainCon.Value = currentThread;
                return _mainCon;
            }
        }

        /// <summary>
        /// 释放一个连接以供并行操作共享。
        /// </summary>
        /// <param name="con">要释放的数据库连接。</param>
        public void ReleaseForSharing(IDbConnection con)
        {
            if (con == _mainCon)
            {
                lock (_busyMainCon)
                {
                    _busyMainCon.Value = null;
                    Monitor.PulseAll(_busyMainCon);
                }
            }
            else
            {
                lock (_lock)
                {
                    
                    _busyQueryConList.TryDequeue(out con);
                    _freeQueryConList.Enqueue(con);
                }
            }
        }

        /// <summary>
        /// 关闭数据库连接，并可选地回滚任何挂起的事务。
        /// </summary>
        /// <param name="con">要关闭的数据库连接。</param>
        /// <param name="rollback">指示是否应回滚任何挂起的事务。</param>
        public void CloseConnection(IDbConnection con, bool rollback)
        {
            bool main = con == _mainCon;
            try
            {
                if (rollback)
                {
                    RollbackConnection(main, con);
                }
            }
            finally
            {
                if (main)
                {
                    lock (_busyMainCon)
                    {
                        _busyMainCon.Value = null;
                        Monitor.PulseAll(_busyMainCon);
                    }
                }
                else
                {
                    lock (_lock)
                    {
                        if (_queryConRefMap.TryGetValue(con, out var refCount) && refCount.DecrementAndGet() == 0)
                        {
                            CloseConnection(false, con);
                            _freeQueryConList.TryDequeue(out con);
                            //_freeQueryConList.TryRemove(con, out _);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断给定的连接是否为主连接。
        /// </summary>
        /// <param name="con">要检查的数据库连接。</param>
        /// <returns>如果是主连接则返回 true；否则，返回 false。</returns>
        public bool IsMainConnection(IDbConnection con)
        {
            return con == _mainCon;
        }

        /// <summary>
        /// 获取当前持有的连接数量。
        /// </summary>
        /// <returns>连接的数量。</returns>
        public int GetHoldingConnections()
        {
            lock (_lock)
            {
                return _freeQueryConList.Count + _busyQueryConList.Count + 1;
            }
        }

        /// <summary>
        /// 创建新的数据库连接。
        /// </summary>
        /// <param name="forManager">指示是否为管理器操作创建连接。</param>
        /// <param name="querySQL">将使用连接执行的 SQL 查询。</param>
        /// <returns>数据库连接。</returns>
        protected abstract IDbConnection CreateConnection(bool forManager, string querySQL);

        /// <summary>
        /// 关闭数据库连接。
        /// </summary>
        /// <param name="isMain">指示连接是否为主连接。</param>
        /// <param name="con">要关闭的数据库连接。</param>
        protected abstract void CloseConnection(bool isMain, IDbConnection con);

        /// <summary>
        /// 回滚数据库连接上的任何挂起的事务。
        /// </summary>
        /// <param name="isMain">指示连接是否为主连接。</param>
        /// <param name="con">要回滚事务的数据库连接。</param>
        protected abstract void RollbackConnection(bool isMain, IDbConnection con);

        public abstract DBType GetDBType();
    }

}
