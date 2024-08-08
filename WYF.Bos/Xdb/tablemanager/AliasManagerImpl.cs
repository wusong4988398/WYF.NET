using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.xdb.tablemanager
{
    public class AliasManagerImpl : IAliasManager
    {
        public static  AliasManagerImpl INSTANCE = new AliasManagerImpl();

        public string GetIndexAliasName(string tableName, string oriIndexName)
        {
            throw new NotImplementedException();
        }

        public string GetTableAliasName(string originalName)
        {
            throw new NotImplementedException();
        }

        public string GetTableOriginalName(string aliasName)
        {
            throw new NotImplementedException();
        }
    }
}
