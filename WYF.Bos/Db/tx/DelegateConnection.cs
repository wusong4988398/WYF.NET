using WYF.Bos.Threading;
using WYF.Bos.xdb.datasource;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db.tx
{
    public class DelegateConnection: AbstractParallelConnectionHolder, IDbConnection
    {
        private List<QueryResource> unReleaseResources = new List<QueryResource>();
        private IDbConnection _con;
        private readonly long id;
        private string simpleURL;

        private bool autoCommit = true;

        private readonly bool inTX;
  
        private readonly TXContext ctx;
  

  
       private readonly string routeKey;
  
       private readonly DBConfig dbConfig;
  
       private readonly DBType dbType;
  
       private bool writed = false;
       private AtomicBoolean closed = new AtomicBoolean();
        private int refs = 0;
  
       private readonly bool useForReadOnly;
  
       private List<string> writeSqlList = new List<string>();



       //private readonly ParallelConnectionSupplier pcs;
  
       private List<IDbConnection> pcons;

        public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int ConnectionTimeout => throw new NotImplementedException();

        public string Database => throw new NotImplementedException();

        public ConnectionState State => throw new NotImplementedException();

        public IDbTransaction BeginTransaction()
        {
            return this._con.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this._con.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            this._con.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            this._con.Close();
        }

        public IDbCommand CreateCommand()
        {
            return this._con.CreateCommand();
        }

        public void Dispose()
        {
           this._con.Dispose();
        }

        public void Open()
        {
            this._con.Open();
        }
    }
}
