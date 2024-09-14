using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlSelectInto : SqlObject
    {
        // Fields
        private string intoWord;
        public string new_table;
        private string new_table_orgName;

        // Methods
        public SqlSelectInto()
        {
        }

        public SqlSelectInto(string new_table)
        {
            this.new_table = new_table;
            this.setNewTableOrgName(new_table);
        }

        public override object Clone()
        {
            SqlSelectInto into = new SqlSelectInto(this.new_table);
            into.setIntoWord(this.getIntoWord());
            into.setNewTableOrgName(this.getNew_table_orgName());
            return into;
        }

        public string getIntoWord()
        {
            return this.intoWord;
        }

        public string getNew_table_orgName()
        {
            return this.new_table_orgName;
        }

        public void setIntoWord(string intoWord)
        {
            this.intoWord = intoWord;
        }

        public void setNewTableOrgName(string new_table_orgName)
        {
            this.new_table_orgName = new_table_orgName;
        }
    }






}
