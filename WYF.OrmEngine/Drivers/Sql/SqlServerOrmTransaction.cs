using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;

namespace WYF.OrmEngine.Drivers.Sql
{
    public class SqlServerOrmTransaction : OrmTransactionBase
    {
     
        protected Context _ctx;

       
        public SqlServerOrmTransaction(Context ctx)
        {
            this._ctx = ctx;
        }

        protected override IDbConnection CreateAndOpenConnection()
        {
            return TransactionScopeConnections.GetConnection(DatabaseFactory.CreateDataBase(this._ctx));
        }

        public override IDatabaseTask CreateBulkCopyTask(string tableName, ReadOnlyCollection<DbMetadataColumn> columns, IDataReader dataReader, int level)
        {
            return new KSqlServerBulkCopyTask(this._ctx, tableName, columns, dataReader, level);
        }

        public override IDatabaseTask CreateBulkUpdateTask(string tableName, ReadOnlyCollection<DbMetadataColumn> readOnlyCollection, IDataReader dataReader, DbMetadataColumn pkColumn, object[] oids, int level)
        {
            return new KSqlServerBulkUpdateTask(this._ctx, tableName, readOnlyCollection, dataReader, pkColumn, oids, level);
        }

        protected override SqlTask CreateSqlTask()
        {
            //return new KSqlForSqlServerTask(this._ctx);
            return null;
        }

        protected override Func<object, object> GetConverter(DbMetadataColumn col)
        {
            DbType dbType = (DbType)col.DbType;
            switch (dbType)
            {
                case DbType.Int16:
                case DbType.Int32:
                    if (!(col.ClrType == typeof(bool)))
                    {
                        return base.GetConverter(col);
                    }
                    return new Func<object, object>(OLEDbDriver.BoolToInt);
            }
            if (dbType == DbType.StringFixedLength)
            {
                if (col.ClrType == typeof(bool))
                {
                    return new Func<object, object>(OLEDbDriver.BoolToChar);
                }
                if (Nullable.GetUnderlyingType(col.ClrType) == typeof(bool))
                {
                    return new Func<object, object>(OLEDbDriver.NullBoolToChar);
                }
            }
            return base.GetConverter(col);
        }

        // Properties
        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                return null;
            }
        }

        public override bool ExTableHaveRelitionField
        {
            get
            {
                return true;
            }
        }

        public override IsolationLevel IsolationLevel
        {
            get
            {
                return this._ctx.TransIsolationLevel;
            }
        }

        public override bool SupportsBulkCopy
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsBulkUpdate
        {
            get
            {
                return true;
            }
        }
    }
}
