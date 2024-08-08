using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.xdb.tablemanager
{
    public interface IAliasManager
    {
        static IAliasManager Get()
        {
            return AliasManagerImpl.INSTANCE;
        }

        string GetTableAliasName(string originalName);

        string GetTableOriginalName(string aliasName);

        string GetIndexAliasName(string tableName, string oriIndexName);
    }
}
