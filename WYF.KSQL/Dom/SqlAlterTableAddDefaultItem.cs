using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlAlterTableAddDefaultItem : SqlAlterTableItem
    {
        // Fields
        public string columnName;
        private string defaultWord;
        private string forWord;
        public SqlExpr value;

        // Methods
        public SqlAlterTableAddDefaultItem()
        {
        }

        public SqlAlterTableAddDefaultItem(SqlExpr defaultValue, string columnName)
        {
            this.value = defaultValue;
            this.columnName = columnName;
        }

        public override object Clone()
        {
            SqlAlterTableAddDefaultItem item = new SqlAlterTableAddDefaultItem
            {
                value = (SqlExpr)this.value.Clone(),
                columnName = string.Copy(this.columnName)
            };
            item.setDefaultWord(this.getDefaultWord());
            item.setItemWord(this.getItemWord());
            item.setOpenBrace(base.isOpenBrace());
            item.setForWord(this.getForWord());
            return item;
        }

        public string getDefaultWord()
        {
            if ((this.defaultWord != null) && (this.defaultWord.Trim().Length != 0))
            {
                return this.defaultWord;
            }
            return "DEFAULT";
        }

        public string getForWord()
        {
            if ((this.forWord != null) && (this.forWord.Trim().Length != 0))
            {
                return this.forWord;
            }
            return "FOR";
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

        public void setDefaultWord(string defaultWord)
        {
            this.defaultWord = defaultWord;
        }

        public void setForWord(string forWord)
        {
            this.forWord = forWord;
        }
    }






}
