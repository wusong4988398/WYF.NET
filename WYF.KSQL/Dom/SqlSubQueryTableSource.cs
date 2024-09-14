using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlSubQueryTableSource : SqlTableSourceBase
    {
        // Fields
        public SqlSelectBase subQuery;

        // Methods
        public SqlSubQueryTableSource()
        {
        }

        public SqlSubQueryTableSource(SqlSelectBase subQuery)
        {
            this.subQuery = subQuery;
        }

        public override object Clone()
        {
            SqlSubQueryTableSource source = new SqlSubQueryTableSource();
            if (this.subQuery != null)
            {
                source.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            return source;
        }
    }






}
