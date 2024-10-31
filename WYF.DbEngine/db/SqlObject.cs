using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine.db
{
    public class SqlObject
    {
        private StringBuilder _sql = new StringBuilder(128);

        private List<SqlParameter> _params = new List<SqlParameter>();

        public SqlObject() { }

        public SqlObject(string sql, SqlParameter[] paramsArray)
        {
            SetSql(sql);
            SetParams(paramsArray);
        }

        public string Sql
        {
            get { return _sql.ToString(); }
        }

        public void RemoveLast(int length)
        {
            _sql.Length = _sql.Length - length;
        }

        public void SetSql(string sql)
        {
            _sql.Clear();
            if (sql != null)
            {
                _sql.Append(sql);
            }
        }

        public SqlParameter[] Params
        {
            get { return _params.ToArray(); }
        }

        public void SetParams(SqlParameter[] paramsArray)
        {
            _params.Clear();
            if (paramsArray != null && paramsArray.Length > 0)
            {
                _params.AddRange(paramsArray);
            }
        }

        public SqlObject AppendSql(char ch)
        {
            _sql.Append(ch);
            return this;
        }

        public SqlObject AppendSql(string sql)
        {
            _sql.Append(sql);
            return this;
        }

        public SqlObject AddParam(SqlParameter param)
        {
            _params.Add(param);
            return this;
        }

        public void ClearParams()
        {
            _params.Clear();
        }

        public override string ToString()
        {
            return $"{_sql}{string.Join(", ", _params)}";
        }
    }
}
