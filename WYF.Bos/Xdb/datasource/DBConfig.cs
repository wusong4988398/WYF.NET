using Mysqlx.Crud;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.xdb.datasource
{
    public  class DBConfig
    {
        private static readonly AtomicInteger idSeq=new AtomicInteger(0);
        private static readonly ConcurrentDictionary<string, Account> AccountCacheMap = new ConcurrentDictionary<string, Account>();
        public DBConfig()
        {
            this.Id = idSeq.IncrementAndGet();
        }
        public string Schema { get; set; }
        public string Mode { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public  int Id { get; }
        public string Driver { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public DBType DbType { get; set; }
        public int PoolInitialSize { get; set; } = 2;
        public int PoolMaxIdle { get; set; } = 1;
        public int PoolMinIdle { get; set; } = 1;
        public int PoolMaxActive { get; set; } = 100;
        public int PoolMaxWait { get; set; } = 60000;
        public string TenantId { get; set; }
        public string RouteKey { get; set; }
        public string AccountId { get; set; }
        public bool ReadOnly { get; set; }
        public int LoadFactor { get; set; } = 0;
        public bool IsCluster { get; set; }
        public string ClusterDbUrl { get; set; }
        public string SharingId { get; set; }
        public string DbShardedIdentity { get; set; }
      
    }
}
