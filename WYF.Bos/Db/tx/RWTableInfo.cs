
using WYF.Bos.ksql;
using WYF.Bos.ksql.parser;
using WYF.Bos.ksql.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.db.tx
{
    public class RWTableInfo
    {
        
        private static ThreadLocal<Dictionary<string, RWTableInfo>> threadCacheMap = new ThreadLocal<Dictionary<string, RWTableInfo>>();
        private bool writedSQL = true;
        private bool isDDL = false;
        public string MainTable { get; set; }
        public HashSet<string> AllTables { get; set; } =new HashSet<string>();
        public static RWTableInfo ParseRWTableInfo(string sql)
        {
            return threadCacheMap.Value.GetOrAdd(sql, () => {
                RWTableInfo ret = new RWTableInfo();
                ret.writedSQL = true;
                Lexer lexer = new Lexer(sql, true);
                Token t = lexer.Next();
                string cmd = t.value.ToLower();
                switch (cmd)
                {
                    case "select":
                        ret.isDDL = false;
                        ret.writedSQL = false;
                        break;
                    case "delete":
                    case "update":
                    case "insert":
                        ret.isDDL = false;
                        break;
                    default:
                        ret.isDDL = true;
                        break;
                }
                switch (cmd)
                {
                    case "select":
                        do
                        {
                            t = lexer.Next();
                            if (t.type != 3)
                                continue;
                            String lv = t.value.ToLower();
                            if ("from".Equals(lv))
                            {
                                ret.MainTable = UnwrapTableName(lexer.Next().OrgValue.ToLower());
                                ret.AllTables.Add(ret.MainTable);
                            }
                            else if ("join".Equals(lv))
                            {
                                ret.AllTables.Add(UnwrapTableName(lexer.Next().OrgValue.ToLower()));
                            }
                        } while (t.type != 12);
                        break;
                    case "delete":
                        while (true)
                        {
                            t = lexer.Next();
                            if (t.type == 3 && "from".Equals(t.value.ToLower()))
                            {
                                ret.MainTable = UnwrapTableName(lexer.Next().OrgValue.ToLower());
                                ret.AllTables.Add(ret.MainTable);
                                break;
                            }
                            string tableName = t.OrgValue.ToLower();
                            if ("*".Equals(tableName))
                            {
                                if (t.type == 12)
                                    break;
                                continue;
                            }
                            ret.MainTable = UnwrapTableName(tableName);
                            ret.AllTables.Add(ret.MainTable);
                            break;
                        }
                        break;
                    case "update":
                        ret.MainTable = UnwrapTableName(lexer.Next().OrgValue.ToLower());
                        ret.AllTables.Add(ret.MainTable);
                        break;
                    case "insert":
                        t = lexer.Next();
                        if ("into".Equals(t.value.ToLower()))
                        {
                            ret.MainTable = UnwrapTableName(lexer.Next().OrgValue.ToLower());
                            ret.AllTables.Add(ret.MainTable);
                        }
                        break;
                }
                return ret;
            });
        }


        private static string UnwrapTableName(string tableName)
        {
            
            if (tableName.ElementAt(0) == '[')
                tableName = tableName.Substring(1, tableName.Length - 1).Trim();
            return tableName;
        }

    }
}
