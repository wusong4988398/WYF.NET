
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.dataentity
{
    public class TableAliasGenner
    {
        private Dictionary<string, string> aliasDict = new Dictionary<string, string>();

        public string GetTableName(string name, bool withAlias)
        {
            if (withAlias)
                return name + ' ' + GetAlias(name);
            return name;
        }

        public string GetAlias(string name)
        {
            
            string simpleAlias = this.aliasDict.GetValueOrDefault(name.ToLower(), "");
            if (string.IsNullOrEmpty(simpleAlias))
            {
                simpleAlias = "T" + (this.aliasDict.Count + 1);
                this.aliasDict[name.ToLower()]= simpleAlias;
            }
            return simpleAlias;
        }
    }
}
