using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlAlterTableDropItem : SqlAlterTableItem
    {
        // Fields
        public ArrayList columnDefItemList;
        public ArrayList constraintItemList;

        // Methods
        public SqlAlterTableDropItem()
        {
            this.columnDefItemList = new ArrayList();
            this.constraintItemList = new ArrayList();
        }

        public SqlAlterTableDropItem(int columnDefItemListSize, int constraintItemListSize)
        {
            this.columnDefItemList = new ArrayList(columnDefItemListSize);
            this.constraintItemList = new ArrayList(constraintItemListSize);
        }

        public override object Clone()
        {
            SqlAlterTableDropItem item = new SqlAlterTableDropItem();
            if (this.columnDefItemList != null)
            {
                int count = this.columnDefItemList.Count;
                for (int i = 0; i < count; i++)
                {
                    string str = (string)this.columnDefItemList[i];
                    item.columnDefItemList.Add(str);
                }
            }
            if (this.constraintItemList != null)
            {
                int num3 = this.constraintItemList.Count;
                for (int j = 0; j < num3; j++)
                {
                    string str2 = (string)this.constraintItemList[j];
                    item.constraintItemList.Add(str2);
                }
            }
            item.setItemWord(this.getItemWord());
            return item;
        }
    }


   



}
