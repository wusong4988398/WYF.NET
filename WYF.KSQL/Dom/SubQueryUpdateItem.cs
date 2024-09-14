using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SubQueryUpdateItem : AbstractUpdateItem
    {
        // Fields
        public ArrayList columnList;
        public SqlSelectBase subQuery;

        // Methods
        public SubQueryUpdateItem()
        {
            this.columnList = new ArrayList();
        }

        public SubQueryUpdateItem(int columnListSize)
        {
            this.columnList = new ArrayList(columnListSize);
        }

        public override object Clone()
        {
            int count = this.columnList.Count;
            SubQueryUpdateItem item = new SubQueryUpdateItem(count);
            for (int i = 0; i < count; i++)
            {
                string str = (string)this.columnList[i];
                item.columnList.Add(str);
            }
            return item;
        }
    }





}
