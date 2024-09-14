using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace WYF.DbEngine
{
    /// <summary>
    /// 管理事务范围内的数据库连接。它通过一个嵌套字典结构来存储和管理这些连接，并确保在事务完成后正确地释放所有连接。
    /// 这种设计确保了事务范围内的连接管理是线程安全的，并且能够在事务完成时自动清理资源，避免资源泄露。
    /// 这种模式在需要跨多个数据库操作且希望在事务完成时自动释放资源的场景中非常有用。它通过将连接与事务绑定，确保在事务提交或回滚时，
    /// 所有相关的连接都会被正确地关闭。
    /// </summary>
    public static class TransactionScopeConnections
    {

        private static readonly Dictionary<Transaction, Dictionary<string, DbConnection>> transactionConnections = new Dictionary<Transaction, Dictionary<string, DbConnection>>();

        /// <summary>
        /// 用于获取一个数据库连接。它根据当前是否有事务在运行来决定如何获取连接
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(IDatabase db)
        {
            Dictionary<string, DbConnection> dictionary;
            DbConnection newOpenConnection;
            Transaction current = Transaction.Current;
            if (current == null)//没有事务
            {
                if (SessionScope.Current == null)
                {
                    return null;
                }
                //如果不匹配，则创建一个新的连接并设置到 SessionScope.Current 中
                if ((SessionScope.Current.Connection == null) || !SessionScope.Current.connectionString.Equals(db.ConnectionString))
                {
                    SessionScope.Current.Connection = db.GetNewOpenConnection();
                    SessionScope.Current.connectionString = db.ConnectionString;
                }
                return SessionScope.Current.Connection;
            }
            //有事务
            lock (transactionConnections)
            {
                if (!transactionConnections.TryGetValue(current, out dictionary))
                {
                    dictionary = new Dictionary<string, DbConnection>();
                    transactionConnections.Add(current, dictionary);
                }
            }
            lock (dictionary)
            {
                if (dictionary.TryGetValue(db.ConnectionString, out newOpenConnection))
                {
                    return newOpenConnection;
                }
                if (((SessionScope.Current != null) && (SessionScope.Current.Connection != null)) && SessionScope.Current.connectionString.Equals(db.ConnectionString))
                {
                    return SessionScope.Current.Connection;
                }
                newOpenConnection = db.GetNewOpenConnection();
                //TransactionCompleted这个方法作为事务完成事件的事件处理程序，用于在事务完成后释放所有的连接
                current.TransactionCompleted += new TransactionCompletedEventHandler(TransactionScopeConnections.OnTransactionCompleted);
                dictionary.Add(db.ConnectionString, newOpenConnection);
            }
            return newOpenConnection;
        }

        private static void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            Dictionary<string, DbConnection> dictionary;
            lock (transactionConnections)
            {
                if (!transactionConnections.TryGetValue(e.Transaction, out dictionary))
                {
                    return;
                }
                transactionConnections.Remove(e.Transaction);
            }
            lock (dictionary)
            {
                foreach (DbConnection connection in dictionary.Values)
                {
                    connection.Dispose();
                }
            }
        }
    }
}
