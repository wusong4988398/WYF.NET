using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine;

namespace WYF.OrmEngine.Drivers.Sql
{
    public  class KSqlForSqlServerTask : OLEDbDriver.KSqlTask
    {
        // Methods
        public KSqlForSqlServerTask(Context ctx) : base(ctx)
        {
        }

        public override object AddParamter(string name, DbType dbType, object value, out string paramterName)
        {
            value = value ?? DBNull.Value;
            paramterName = "@" + name;
            SqlParam item = new SqlParam(paramterName, (KDbType)dbType, value);
            base.Parameters.Add(item);
            return item;
        }

        public override object AddUdtParamter(string name, DbType dbType, object value, out string paramterName)
        {
            value = value ?? DBNull.Value;
            paramterName = "@" + name;
            SqlParam item = SqlParam.CreateUdtParamter(paramterName, (KDbType)dbType, value);
            base.Parameters.Add(item);
            return item;
        }

        public override void Execute(IDbConnection con, IDbTransaction tran)
        {
            DBUtils.Execute(base._ctx, base.SQL, base.Parameters,false);
        }
    }
}
