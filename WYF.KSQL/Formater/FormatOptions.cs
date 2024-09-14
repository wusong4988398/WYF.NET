using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Formater
{
    public class FormatOptions
    {
        // Fields
        private string dbSchema;
        private bool nologging = true;
        private string tableSchema;
        private string[] tempTableSpaces;

        // Methods
        public FormatOptions Clone()
        {
            return new FormatOptions { nologging = this.nologging, dbSchema = this.dbSchema, tableSchema = this.tableSchema, tempTableSpaces = this.tempTableSpaces };
        }

        public string GetDbSchema()
        {
            return this.dbSchema;
        }

        public string GetTableSchema()
        {
            return this.tableSchema;
        }

        public string GetTempTableSpace()
        {
            if ((this.tempTableSpaces == null) || (this.tempTableSpaces.Length == 0))
            {
                return null;
            }
            int index = (int)(new Random().NextDouble() * this.tempTableSpaces.Length);
            return this.tempTableSpaces[index];
        }

        public string[] GetTempTableSpaces()
        {
            return this.tempTableSpaces;
        }

        public bool IsNologging()
        {
            return this.nologging;
        }

        public void SetDbSchema(string dbSchema)
        {
            if (dbSchema != null)
            {
                this.dbSchema = dbSchema.ToUpper();
            }
        }

        public void SetNologging(bool nologging)
        {
            this.nologging = nologging;
        }

        public void SetTableSchema(string tableSchema)
        {
            this.tableSchema = tableSchema;
        }

        public void SetTempTableSpaces(string[] temptableSpaces)
        {
            this.tempTableSpaces = temptableSpaces;
        }
    }





}
