using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine
{
    public class ConnectionWrapper : IDisposable
    {
        private readonly DbConnection connection;
        private readonly bool disposeConnection;
        public ConnectionWrapper(DbConnection connection, bool disposeConnection)
        {
            this.connection = connection;
            this.disposeConnection = disposeConnection;
        }
        public void Dispose()
        {
            if (this.disposeConnection)
            {
                this.connection.Dispose();
            }
        }
        public DbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }
    }
}
