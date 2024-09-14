using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlDropTriggerStmt : SqlStmt
    {
        // Fields
        private string dropWord;
        private string orgTrggerName;
        public string trggerName;

        // Methods
        public SqlDropTriggerStmt() : base(0x2c)
        {
        }

        public SqlDropTriggerStmt(string trggerName) : base(0x2c)
        {
            this.trggerName = trggerName;
        }

        public SqlDropTriggerStmt(string trggerName, string orgTrggerName, string dropWord) : base(0x2c)
        {
            this.trggerName = trggerName;
            this.orgTrggerName = orgTrggerName;
            this.dropWord = dropWord;
        }

        public string GetDropWord()
        {
            if (string.IsNullOrEmpty(this.dropWord))
            {
                return "DROP TRIGGER";
            }
            return this.dropWord;
        }

        public string GetOrgTrggerName()
        {
            if (string.IsNullOrEmpty(this.orgTrggerName))
            {
                return this.trggerName;
            }
            return this.orgTrggerName;
        }

        public void SetDropWord(string dropWord)
        {
            this.dropWord = dropWord;
        }

        public void SetOrgTrggerName(string orgTrggerName)
        {
            this.orgTrggerName = orgTrggerName;
        }
    }






}
