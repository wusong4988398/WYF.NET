
using WYF.Bos.Orm;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;
namespace WYF.Bos.db
{
  
        /// <summary>
        /// 
        /// 数据库连接工厂类
        /// </summary>
        public class ConnectionFactory
        {
            /// <summary>
            /// 获取数据库连接
            /// </summary>
            /// <param name="dbtype">数据库类型</param>
            /// <param name="conStr">数据库连接字符串</param>
            /// <returns>数据库连接</returns>
            public static IDbConnection CreateConnection(string dbtype, string strConn)
            {
                if (dbtype.IsNullOrWhiteSpace())
                    throw new ArgumentNullException("获取数据库连接居然不传数据库类型，你想上天吗？");
                if (strConn.IsNullOrWhiteSpace())
                    throw new ArgumentNullException("获取数据库连接居然不传数据库类型，你想上天吗？");
                var dbType = GetDataBaseType(dbtype);
                return CreateConnection(dbType, strConn);
            }

            /// <summary>
            /// 获取数据库连接
            /// </summary>
            /// <param name="dbType">数据库类型</param>
            /// <param name="conStr">数据库连接字符串</param>
            /// <returns>数据库连接</returns>
            public static IDbConnection CreateConnection(DatabaseType dbType, string strConn)
            {
                IDbConnection connection = null;
                if (strConn.IsNullOrWhiteSpace())
                    throw new ArgumentNullException("获取数据库连接居然不传数据库类型，你想上天吗？");

                switch (dbType)
                {
                    case DatabaseType.MS_SQL_Server:
                        connection = new SqlConnection(strConn);
                        break;
                    case DatabaseType.MySQL:
                        connection = new MySqlConnection(strConn);
                        break;
                    case DatabaseType.PostgreSQL:
                        connection = new NpgsqlConnection(strConn);
                        break;
                    default:
                        throw new ArgumentNullException($"这是我的错，还不支持的{dbType.ToString()}数据库类型");

                }
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection;
            }

            /// <summary>
            /// 转换数据库类型
            /// </summary>
            /// <param name="dbtype">数据库类型字符串</param>
            /// <returns>数据库类型</returns>
            public static DatabaseType GetDataBaseType(string dbtype)
            {
                if (dbtype.IsNullOrWhiteSpace())
                    throw new ArgumentNullException("获取数据库连接居然不传数据库类型，你想上天吗？");
                DatabaseType returnValue = DatabaseType.MS_SQL_Server;
                foreach (DatabaseType dbType in Enum.GetValues(typeof(DatabaseType)))
                {
                    if (dbType.ToString().Equals(dbtype, StringComparison.OrdinalIgnoreCase))
                    {
                        returnValue = dbType;
                        break;
                    }
                }
                return returnValue;
            }


        }
  
}
