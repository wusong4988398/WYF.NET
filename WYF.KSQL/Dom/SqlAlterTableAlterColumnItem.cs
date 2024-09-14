using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlAlterTableAlterColumnItem : SqlAlterTableItem
    {
        // Fields
        public SqlColumnDef columnDef;

        // Methods
        public SqlAlterTableAlterColumnItem()
        {
        }

        public SqlAlterTableAlterColumnItem(SqlColumnDef columnDef)
        {
            this.columnDef = columnDef;
        }

        public override object Clone()
        {
            SqlAlterTableAlterColumnItem item = null;
            if (this.columnDef != null)
            {
                item = new SqlAlterTableAlterColumnItem((SqlColumnDef)this.columnDef.Clone());
            }
            else
            {
                item = new SqlAlterTableAlterColumnItem();
            }
            item.setItemWord(this.getItemWord());
            return item;
        }

        public override string getItemWord()
        {
            string str = base.getItemWord();
            if (str == null)
            {
                str = "ALTER COLUMN";
            }
            return str;
        }
    }






}
