using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlUnionSelect : SqlSelectBase
    {
        // Fields
        public SqlSelectBase left;
        public int option;
        public SqlSelectBase right;
        public static int Union = 0;
        public static int UnionAll = 1;

        // Methods
        public SqlUnionSelect()
        {
            this.option = Union;
        }

        public SqlUnionSelect(SqlSelectBase left, SqlSelectBase right, int option)
        {
            this.option = Union;
            this.left = left;
            this.right = right;
            this.option = option;
        }

        public override object Clone()
        {
            SqlUnionSelect select = new SqlUnionSelect();
            if (this.left != null)
            {
                select.left = (SqlSelectBase)this.left.Clone();
            }
            if (this.left != null)
            {
                select.right = (SqlSelectBase)this.right.Clone();
            }
            select.option = this.option;
            return select;
        }
    }






}
