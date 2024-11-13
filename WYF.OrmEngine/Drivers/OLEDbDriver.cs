using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;
using WYF.Form.DataEntity;
using WYF.OrmEngine.dataManager;
using WYF.OrmEngine.Drivers.Sql;

namespace WYF.OrmEngine.Drivers
{
    public sealed class OLEDbDriver : DbDriverBase
    {

        private Context ctx;


        public OLEDbDriver(Context ctx)
        {
            this.ctx = ctx;
        }

        public override IOrmTransaction BeginTransaction(IDbTransaction dbTransaction = null)
        {
            return CreateOrmTransaction(this.ctx);
        }

        internal static object BoolToChar(object obj)
        {
            if ((bool)obj)
            {
                return '1';
            }
            return '0';
        }

        internal static object BoolToInt(object obj)
        {
            if ((bool)obj)
            {
                return 1;
            }
            return 0;
        }

        internal static object CharToBool(object obj)
        {
            return ((obj != DBNull.Value) && (obj.ToString() == "1"));
        }

        internal static object CharToDateTime(object obj)
        {
            //DateTime minSystemDateTime = KDTimeZone.MinSystemDateTime;
            DateTime minSystemDateTime=DateTime.MinValue;
            DateTime.TryParse(Convert.ToString(obj),  out minSystemDateTime);
            return minSystemDateTime;
        }

        internal static object CharToNullBool(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }
            string str = obj as string;
            return (str == "1");
        }

        public static IOrmTransaction CreateOrmTransaction(Context ctx)
        {
            if (ctx.DatabaseType == DatabaseType.MS_SQL_Server)
            {
                return new SqlServerOrmTransaction(ctx);
            }
            if (((ctx.DatabaseType == DatabaseType.Oracle) || (ctx.DatabaseType == DatabaseType.Oracle10)) || (ctx.DatabaseType == DatabaseType.Oracle9))
            {
               // return new OracleOrmTransaction(ctx);
            }
            if (ctx.DatabaseType == DatabaseType.MySQL)
            {
                //return new MySqlOrmTransaction(ctx);
            }
            if (ctx.DatabaseType == DatabaseType.PostgreSQL)
            {
                //new PostgreSqlOrmTransaction(ctx);

            }
            throw new NotSupportedException("新的数据库类型需要补充");
        }

        protected override void ExecuteReader(SelectCallback callback, IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, IList<StringBuilder> selectSqls, ReadWhere where, OperateOption option)
        {
            switch (this.ctx.DatabaseType)
            {
                case DatabaseType.MS_SQL_Server:
                case DatabaseType.MySQL:
                    using (IDataReader reader = new KSql4SQLDataReader(selectSqls, this.ctx, where, tablesSchema, rootTable))
                    {
                        callback(reader);
                        return;
                    }
                case DatabaseType.Oracle:
                case DatabaseType.Oracle10:
                case DatabaseType.Oracle9:
                    //using (IDataReader reader2 = new KSql4OracleDataReader(selectSqls, this.ctx, where, tablesSchema, rootTable))
                    //{
                    //    callback(reader2);
                    //    return;
                    //}
                case DatabaseType.PostgreSQL:
                    //using (IDataReader reader3 = new KSql4PostgreSqlDataReader(selectSqls, this.ctx, where, tablesSchema, rootTable))
                    //{
                    //    callback(reader3);
                    //    return;
                    //}
                default:
                    throw new NotSupportedException("新的数据库类型需要补充");
            }
        }
        internal static object NullBoolToChar(object obj)
        {
            bool? nullable = (bool?)obj;
            if (nullable.HasValue)
            {
                return (nullable.Value ? '1' : '0');
            }
            return DBNull.Value;
        }


        internal abstract class KSqlTask : SqlTask
        {

            protected Context _ctx;
            private List<SqlParam> _parameters;


            protected KSqlTask(Context ctx)
            {
                this._ctx = ctx;
            }

            public override void AddParamters(object[] paramters)
            {
                this.Parameters.Capacity = paramters.Length;
                foreach (SqlParam param in paramters)
                {
                    this._parameters.Add(param);
                }
            }

            public override void Execute(IDbConnection con, IDbTransaction tran)
            {
                DBUtils.Execute(this._ctx, base.SQL, this.Parameters, false);
            }

            // Properties
            public List<SqlParam> Parameters
            {
                get
                {
                    if (this._parameters == null)
                    {
                        this._parameters = new List<SqlParam>();
                    }
                    return this._parameters;
                }
            }
        }
    }
}
