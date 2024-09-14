using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlDropTableStmt : SqlStmt
    {
        // Fields
        private string dropWord;
        private string orgTableName;
        public string tableName;

        // Methods
        public SqlDropTableStmt() : base(0x2b)
        {
        }

        public SqlDropTableStmt(string tableName) : base(0x2b)
        {
            this.tableName = tableName;
        }

        public SqlDropTableStmt(string tableName, string orgTableName, string dropWord) : base(0x2b)
        {
            this.tableName = tableName;
            this.orgTableName = orgTableName;
            this.dropWord = dropWord;
        }

        public string GetDropWord()
        {
            if (string.IsNullOrEmpty(this.dropWord))
            {
                return "DROP TABLE";
            }
            return this.dropWord;
        }

        public string GetOrgTableName()
        {
            if (string.IsNullOrEmpty(this.orgTableName))
            {
                return this.tableName;
            }
            return this.orgTableName;
        }

        public void setDropWord(string dropWord)
        {
            this.dropWord = dropWord;
        }

        public void SetOrgTableName(string orgTableName)
        {
            this.orgTableName = orgTableName;
        }
    }





}
