using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlCreateIndexStmt : SqlStmt
    {
        // Fields
        private string createIndexWord;
        public string indexName;
        public bool isCluster;
        public bool isUnique;
        public IList itemList;
        private string onWord;
        public string tableName;

        // Methods
        public SqlCreateIndexStmt() : base(0x16)
        {
            this.itemList = new ArrayList();
        }

        public SqlCreateIndexStmt(int indexStmtEnum) : base(0x16)
        {
            this.itemList = new ArrayList();
            if (indexStmtEnum == 0)
            {
                this.isUnique = true;
            }
            else if (1 == indexStmtEnum)
            {
                this.isCluster = true;
            }
            else if (10 == indexStmtEnum)
            {
                this.isUnique = true;
                this.isCluster = true;
            }
        }

        public SqlCreateIndexStmt(string indexName) : base(0x16)
        {
            this.itemList = new ArrayList();
            this.indexName = indexName;
        }

        public SqlCreateIndexStmt(string indexName, int indexStmtEnum) : base(0x16)
        {
            this.itemList = new ArrayList();
            this.indexName = indexName;
            if (indexStmtEnum == 0)
            {
                this.isUnique = true;
            }
            else if (1 == indexStmtEnum)
            {
                this.isCluster = true;
            }
            else if (10 == indexStmtEnum)
            {
                this.isUnique = true;
                this.isCluster = true;
            }
        }

        public string GetCreateIndexWord()
        {
            if (string.IsNullOrEmpty(this.createIndexWord))
            {
                return "CREATE INDEX";
            }
            return this.createIndexWord;
        }

        public string GetOnWord()
        {
            if (string.IsNullOrEmpty(this.onWord))
            {
                return "ON";
            }
            return this.onWord;
        }

        public void SetCreateIndexWord(string createIndexWord)
        {
            this.createIndexWord = createIndexWord;
        }

        public void SetOnWord(string onWord)
        {
            this.onWord = onWord;
        }
    }


 



}
