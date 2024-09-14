using WYF.Bos.Threading;
using WYF.Bos.xdb.datasource;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace WYF.Bos.db.tx
{
    using Org.BouncyCastle.Asn1;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using WYF.Bos.Db.tx;

    public sealed class DelegateConnection : AbstractParallelConnectionHolder
    {

        private static long idSeq = 0;

        private readonly long id;
        private string simpleURL;
        private bool autoCommit = true;
        private readonly bool inTX;
        private readonly TXContext ctx;
        private readonly IDbConnection con;
        private readonly string routeKey;
        private readonly DBConfig dbConfig;
        private readonly DBType dbType;
        private bool writed = false;
        private bool closed = false;
        private int refCount = 0;
        private readonly bool useForReadOnly;
        private List<string> writeSqlList = new List<string>();
        private List<QueryResource> unReleaseResources = new List<QueryResource>();
        private readonly IParallelConnectionSupplier pcs;
        private List<IDbConnection> pcons = new List<IDbConnection>();
        public DelegateConnection(TXContext ctx, IDbConnection con, string routeKey, bool readOnly, DBConfig dbConfig, IParallelConnectionSupplier pcs)
        {
            this.id = Interlocked.Increment(ref idSeq);
            this.ctx = ctx;
            this.con = con;
            this.routeKey = routeKey;
            this.dbConfig = dbConfig;
            this.useForReadOnly = readOnly;
            this.inTX = ctx.Propagation != Propagation.NOT_SUPPORTED;
            this.pcs = pcs;

            switch (dbConfig.DbType)
            {
                case DBType.MySQL:
                    this.dbType = DBType.MySQL;
                    break;
                case DBType.Oracle:
                    this.dbType = DBType.Oracle;
                    break;
                case DBType.DM:
                    this.dbType = DBType.DM;
                    break;
                case DBType.PostgreSQL:
                    this.dbType = DBType.PostgreSQL;
                    break;
                case DBType.GS:
                    this.dbType = DBType.GS;
                    break;
                case DBType.GS100:
                    this.dbType = DBType.GS100;
                    break;
                case DBType.SQLServer:
                    this.dbType = DBType.SQLServer;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported database type: {dbConfig.DbType}");
            }
        }
        public override DBType GetDBType()
        {
            throw new NotImplementedException();
        }

        protected override void CloseConnection(bool isMain, IDbConnection con)
        {
            con.Close();
        }

        protected override IDbConnection CreateConnection(bool main, string querySQL)
        {
            if (main || querySQL == null)
            {
               // return this;
            }
            var si = RWTableInfo.ParseRWTableInfo(querySQL);
            var rc = RequestContextInfo.Get();
            //if (!TX.InTX() || TX.CanQueryOnReadOnlyDB(rc.GetTenantId(), this.RouteKey, rc.GetAccountId(), si.GetAllTables().ToArray()))
            //{
            //    var con = this.pcs.GetConnection();

            //    this.pcons.Add(con);

            //    return con;
            //}
            //return this;
            return null;
        }

        protected override void RollbackConnection(bool isMain, IDbConnection con)
        {
            throw new NotImplementedException();
        }
    }
}
