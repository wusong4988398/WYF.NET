using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlAlterTableAddItem : SqlAlterTableItem
    {
        // Fields
        public ArrayList columnDefItemList;
        public ArrayList constraintItemList;

        // Methods
        public SqlAlterTableAddItem()
        {
            this.columnDefItemList = new ArrayList();
            this.constraintItemList = new ArrayList();
        }

        public SqlAlterTableAddItem(SqlTableConstraint constraint)
        {
            this.columnDefItemList = new ArrayList(0);
            this.constraintItemList = new ArrayList(1);
            this.constraintItemList.Add(constraint);
        }

        public SqlAlterTableAddItem(int columnDefItemListSize, int constraintItemListSize)
        {
            this.columnDefItemList = new ArrayList(columnDefItemListSize);
            this.constraintItemList = new ArrayList(constraintItemListSize);
        }

        public override object Clone()
        {
            int count = this.columnDefItemList.Count;
            int constraintItemListSize = this.constraintItemList.Count;
            SqlAlterTableAddItem item = new SqlAlterTableAddItem(count, constraintItemListSize);
            for (int i = 0; i < count; i++)
            {
                SqlColumnDef def = (SqlColumnDef)((SqlColumnDef)this.columnDefItemList[i]).Clone();
                item.columnDefItemList.Add(def);
            }
            for (int j = 0; j < constraintItemListSize; j++)
            {
                SqlTableConstraint constraint = (SqlTableConstraint)((SqlTableConstraint)this.constraintItemList[j]).Clone();
                item.constraintItemList.Add(constraint);
            }
            item.setItemWord(this.getItemWord());
            item.setOpenBrace(base.isOpenBrace());
            return item;
        }

        public override string getItemWord()
        {
            string str = base.getItemWord();
            if ((str != null) && (str.Trim().Length != 0))
            {
                return str;
            }
            return "ADD";
        }
    }






}
