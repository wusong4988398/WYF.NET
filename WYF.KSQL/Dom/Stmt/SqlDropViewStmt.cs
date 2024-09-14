using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlDropViewStmt : SqlStmt
    {
        // Fields
        private string dropWord;
        private string orgViewName;
        public string viewName;

        // Methods
        public SqlDropViewStmt() : base(0x2d)
        {
        }

        public SqlDropViewStmt(string viewName) : base(0x2d)
        {
            this.viewName = viewName;
        }

        public SqlDropViewStmt(string viewName, string orgViewName, string dropWord) : base(0x2d)
        {
            this.viewName = viewName;
            this.orgViewName = orgViewName;
            this.dropWord = dropWord;
        }

        public string GetDropWord()
        {
            if (string.IsNullOrEmpty(this.dropWord))
            {
                return "DROP VIEW";
            }
            return this.dropWord;
        }

        public string GetOrgViewName()
        {
            if (string.IsNullOrEmpty(this.orgViewName))
            {
                return this.viewName;
            }
            return this.orgViewName;
        }

        public void SetDropWord(string dropWord)
        {
            this.dropWord = dropWord;
        }

        public void SetOrgViewName(string orgViewName)
        {
            this.orgViewName = orgViewName;
        }
    }





}
