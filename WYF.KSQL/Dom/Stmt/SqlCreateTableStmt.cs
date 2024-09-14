using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlCreateTableStmt : SqlStmt
    {
        // Fields
        public IList columnList;
        public IList constraintList;
        private string createTableWord;
        public string name;
        public string tableSpace;

        // Methods
        public SqlCreateTableStmt() : base(0x18)
        {
            this.columnList = new ArrayList();
            this.constraintList = new ArrayList();
            this.tableSpace = "";
        }

        public SqlCreateTableStmt(string name) : base(0x18)
        {
            this.columnList = new ArrayList();
            this.constraintList = new ArrayList();
            this.tableSpace = "";
            this.name = name;
        }

        public string GetCreateTableWord()
        {
            if (string.IsNullOrEmpty(this.createTableWord))
            {
                return "CREATE TABLE";
            }
            return this.createTableWord;
        }

        public void SetCreateTableWord(string createTableWord)
        {
            this.createTableWord = createTableWord;
        }
    }






}
