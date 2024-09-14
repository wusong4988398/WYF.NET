using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlCreateViewStmt : SqlStmt
    {
        // Fields
        private string asWord;
        public IList columnList;
        private string createViewWord;
        public string name;
        public SqlSelectBase select;

        // Methods
        public SqlCreateViewStmt() : base(0x1a)
        {
            this.columnList = new ArrayList();
        }

        public SqlCreateViewStmt(SqlSelectBase select, string name) : base(0x1a)
        {
            this.columnList = new ArrayList();
            this.select = select;
            this.name = name;
        }

        public string GetAsWord()
        {
            if (string.IsNullOrEmpty(this.asWord))
            {
                return "AS";
            }
            return this.asWord;
        }

        public string GetCreateViewWord()
        {
            if (string.IsNullOrEmpty(this.createViewWord))
            {
                return "CREATE VIEW";
            }
            return this.createViewWord;
        }

        public void setAsWord(string asWord)
        {
            this.asWord = asWord;
        }

        public void SetCreateViewWord(string createViewWord)
        {
            this.createViewWord = createViewWord;
        }
    }


 



}
