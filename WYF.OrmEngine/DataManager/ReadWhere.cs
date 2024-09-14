using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.DataEntity;
using WYF.DbEngine.db;

namespace WYF.OrmEngine.DataManager
{
    public sealed class ReadWhere
    {
        public ReadWhere(params object[] ids)
        {
            ReadOids = ids ?? throw new ORMArgInvalidException("1", "ORM驱动读取数据时设置读取条件失败，OID不能为空！");
        }

        public ReadWhere(string where)
        {
            WhereSql = where;
        }

        public ReadWhere(string where, List<SqlParameter> sqlParams)
        {
            WhereSql = where;
            SqlParams = sqlParams;
        }

        public bool IsSingleValue
        {
            get => ReadOids != null && ReadOids.Length == 1 && ReadOids[0] != null;
        }

        public object[] ReadOids { get; private set; }

        public string WhereSql { get; private set; }

        public List<SqlParameter> SqlParams { get; private set; }
    }
}
