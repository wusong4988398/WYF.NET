using WYF.Bos.xdb.sharding.sql.parser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.xdb.tablemanager
{
    public class TableName
    {
        
        private static ConcurrentDictionary<string, TableName> cache = new ConcurrentDictionary<string, TableName>();

        private string name;
  
        private  string contextKey;
  
        private string originalName;

        private string aliasName;

        private string suffix;
        private TableName(string name)
        {
            this.contextKey = GetContextKey();
            this.name = name.Substring(this.contextKey.Length + 1);
            Init();
        }
        private void Init()
        {
            int p = this.name.LastIndexOf("$");
            if (p != -1)
            {
                this.originalName = this.aliasName = this.name.Substring(0, p);
                this.suffix = this.name.Substring(p + 1);
                this.originalName = IAliasManager.Get().GetTableOriginalName(this.aliasName);
            }
            else
            {
                this.originalName = this.name;
                this.aliasName = IAliasManager.Get().GetTableAliasName(this.originalName);
            }
        }

        public static TableName Of(string tableName)
        {
            tableName = SQLUtil.UnWrapSQLTableName(tableName).ToLower();
            string key = GetContextKey() + "#" + tableName;
            return cache.GetOrAdd(key, (name) => { return new TableName(name); });
        }

        private static string GetContextKey()
        {
            //return ShardingEngineFactory.Get().getShardingConfigProvider().getContextKey();
            return "bos";
        }

    }
}
