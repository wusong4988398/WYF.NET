using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlAlterTableStmt : SqlStmt
    {
        // Fields
        private string alterWord;
        public SqlAlterTableItem item;
        public IList items;
        public string tableName;

        // Methods
        public SqlAlterTableStmt() : base(0x21)
        {
            this.items = new ArrayList();
            this.SetAlterWord("ALTER TABLE");
        }

        public SqlAlterTableStmt(string tableName) : base(0x21)
        {
            this.tableName = tableName;
            this.items = new ArrayList();
            this.SetAlterWord("ALTER TABLE");
        }

        public override object Clone()
        {
            SqlAlterTableStmt stmt = new SqlAlterTableStmt(this.tableName);
            if (this.item != null)
            {
                stmt.item = (SqlAlterTableItem)this.item.Clone();
            }
            int count = this.items.Count;
            for (int i = 0; i < count; i++)
            {
                stmt.items.Add((SqlAlterTableItem)((SqlAlterTableItem)this.items[i]).Clone());
            }
            stmt.SetAlterWord(this.GetAlterWord());
            return stmt;
        }

        public string GetAlterWord()
        {
            if (string.IsNullOrEmpty(this.alterWord))
            {
                return "ALTER TABLE";
            }
            return this.alterWord;
        }

        public void SetAlterWord(string alterWord)
        {
            this.alterWord = alterWord;
        }
    }




}
