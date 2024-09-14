using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlAlterTableDropDefaultItem : SqlAlterTableItem
    {
        // Fields
        public string columnName;
        private string forWord;

        // Methods
        public SqlAlterTableDropDefaultItem()
        {
        }

        public SqlAlterTableDropDefaultItem(string columnName)
        {
            this.columnName = columnName;
        }

        public override object Clone()
        {
            SqlAlterTableDropDefaultItem item = new SqlAlterTableDropDefaultItem
            {
                columnName = string.Copy(this.columnName)
            };
            item.setForWord(this.getForWord());
            item.setItemWord(this.getItemWord());
            return item;
        }

        public string getForWord()
        {
            if (this.forWord == null)
            {
                return "FOR";
            }
            return this.forWord;
        }

        public void setForWord(string forWord)
        {
            this.forWord = forWord;
        }
    }


  



}
