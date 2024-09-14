using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlExecStmt : SqlStmt
    {
        // Fields
        private string execWord;
        private string orgProcessName;
        public IList paramList;
        public string processName;

        // Methods
        public SqlExecStmt() : base(6)
        {
            this.paramList = new ArrayList();
            this.processName = "";
        }

        public string getExecWord()
        {
            if ((this.execWord != null) && (this.execWord.Length != 0))
            {
                return this.execWord;
            }
            return "EXEC";
        }

        public string getOrgProcessName()
        {
            if ((this.orgProcessName != null) && (this.orgProcessName.Length != 0))
            {
                return this.orgProcessName;
            }
            return this.processName;
        }

        public void setExecWord(string execWord)
        {
            this.execWord = execWord;
        }

        public void setOrgProcessName(string orgProcessName)
        {
            this.orgProcessName = orgProcessName;
        }
    }


   



}
