using WYF.Bos.Orm.Exceptions;
using WYF.Bos.Orm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Drivers
{
    public sealed class ReadWhere
    {
        private bool _isSingleValue;
        private object[] _readOids;
        private string _whereSql;

        public ReadWhere(object[] ids)
        {
            this.ReadOids = ids;
        }

        public ReadWhere(string where)
        {
            this.WhereSql = where;
        }

        public ReadWhere(string where, List<ISqlParam> sqlParams)
        {
            this.WhereSql = where;
            this.SqlParams = sqlParams;
        }

        
        public bool IsSingleValue
        {
            get
            {
                return this._isSingleValue;
            }
        }

        public object[] ReadOids
        {
            get
            {
                return this._readOids;
            }
            set
            {
                if (value == null)
                {
                    throw new ORMArgInvalidException("??????", "ORM驱动读取数据时设置读取条件失败，OID不能为空！");
                }
                this._readOids = value;
                this._isSingleValue = ((this.ReadOids != null) && (this.ReadOids.Length == 1)) && (this.ReadOids[0] != null);
            }
        }

        public List<ISqlParam> SqlParams { get; private set; }

        public string WhereSql
        {
            get
            {
                return this._whereSql;
            }
            set
            {
                this._whereSql = value;
            }
        }
    }
}
