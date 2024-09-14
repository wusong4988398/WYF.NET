using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlDropIndexStmt : SqlStmt
    {
        // Fields
        private string dropWord;
        public string indexName;
        private string orgIndexName;
        private string orgTableName;
        public string tableName;

        // Methods
        public SqlDropIndexStmt() : base(0x2e)
        {
        }

        public SqlDropIndexStmt(string indexName) : base(0x2e)
        {
            this.indexName = indexName;
        }

        public SqlDropIndexStmt(string tableName, string indexName) : base(0x2e)
        {
            this.indexName = indexName;
            this.tableName = tableName;
        }

        public string GetDropWord()
        {
            if (string.IsNullOrEmpty(this.dropWord))
            {
                return "DROP INDEX";
            }
            return this.dropWord;
        }

        public string GetOrgIndexName()
        {
            if (string.IsNullOrEmpty(this.orgIndexName))
            {
                return this.indexName;
            }
            return this.orgIndexName;
        }

        public string GetOrgTableName()
        {
            if (string.IsNullOrEmpty(this.orgTableName))
            {
                return this.tableName;
            }
            return this.orgTableName;
        }

        public void SetDropWord(string dropWord)
        {
            this.dropWord = dropWord;
        }

        public void SetOrgIndexName(string orgIndexName)
        {
            this.orgIndexName = orgIndexName;
        }

        public void SetOrgTableName(string orgTableName)
        {
            this.orgTableName = orgTableName;
        }
    }


 


}
