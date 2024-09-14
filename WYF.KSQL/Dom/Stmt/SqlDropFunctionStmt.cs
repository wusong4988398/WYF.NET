using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlDropFunctionStmt : SqlStmt
    {
        // Fields
        private string dropWord;
        public string functionName;
        private string orgFunctionName;
        private string orgSchema;
        public string schema;

        // Methods
        public SqlDropFunctionStmt() : base(0x29)
        {
        }

        public SqlDropFunctionStmt(string functionName) : base(0x29)
        {
            this.functionName = functionName;
        }

        public SqlDropFunctionStmt(string schema, string functionName) : base(0x29)
        {
            this.schema = schema;
            this.functionName = functionName;
        }

        public string GetDropWord()
        {
            if (string.IsNullOrEmpty(this.dropWord))
            {
                return "DROP FUNCTION";
            }
            return this.dropWord;
        }

        public string GetOrgFunctionName()
        {
            if (string.IsNullOrEmpty(this.orgFunctionName))
            {
                return this.functionName;
            }
            return this.orgFunctionName;
        }

        public string getOrgSchema()
        {
            if (string.IsNullOrEmpty(this.orgSchema))
            {
                return this.schema;
            }
            return this.orgSchema;
        }

        public void setDropWord(string dropWord)
        {
            this.dropWord = dropWord;
        }

        public void SetOrgFunctionName(string orgFunctionName)
        {
            this.orgFunctionName = orgFunctionName;
        }

        public void setOrgSchema(string orgSchema)
        {
            this.orgSchema = orgSchema;
        }
    }






}
