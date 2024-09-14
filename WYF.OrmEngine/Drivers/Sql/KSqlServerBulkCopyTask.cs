using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;

namespace WYF.OrmEngine.Drivers.Sql
{
    internal sealed class KSqlServerBulkCopyTask : SqlServerBulkCopyTask
    {

        private Context _ctx;


        public KSqlServerBulkCopyTask(Context ctx, string tableName, ReadOnlyCollection<DbMetadataColumn> columns, IDataReader dataReader, int level) : base(tableName, columns, dataReader, level)
        {
            this._ctx = ctx;
        }

        public override void Execute(IDbConnection con, IDbTransaction tran)
        {
            bool flag = false;
            if (con == null)
            {
                con = DatabaseFactory.CreateDataBase(this._ctx).GetWrapConnection();
                flag = true;
            }
            try
            {
                base.Execute(con, tran);
            }
            finally
            {
                if (flag && (con != null))
                {
                    con.Close();
                }
            }
        }
    }
}
