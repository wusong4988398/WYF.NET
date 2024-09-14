using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlTableSource : SqlTableSourceBase
    {
        // Fields
        private string hintWord;
        public string lockingHint;
        public string name;
        private string orgName;
        private string withWord;

        // Methods
        public SqlTableSource(string name)
        {
            this.name = name;
            this.setOrgName(name);
        }

        public SqlTableSource(string name, string alias)
        {
            this.name = name;
            base.alias = alias;
            this.setOrgName(name);
            base.setOrgAlias(alias);
        }

        public SqlTableSource(string name, string alias, string lockingHint)
        {
            this.name = name;
            base.alias = alias;
            this.lockingHint = lockingHint;
            this.setOrgName(name);
            base.setOrgAlias(alias);
        }

        public override object Clone()
        {
            SqlTableSource source = new SqlTableSource(this.name, base.alias, this.lockingHint);
            source.setOrgAlias(base.getOrgAlias());
            source.setOrgName(this.getOrgName());
            return source;
        }

        public string getHintWord()
        {
            if ((this.hintWord == null) | (this.hintWord.Length == 0))
            {
                return this.lockingHint;
            }
            return this.hintWord;
        }

        public string getOrgName()
        {
            return this.orgName;
        }

        public string getWithWord()
        {
            if ((this.withWord != null) && (this.withWord.Length != 0))
            {
                return this.withWord;
            }
            return "WITH";
        }

        public void setHintWord(string hintWord)
        {
            this.hintWord = hintWord;
        }

        public void setOrgName(string orgName)
        {
            this.orgName = orgName;
        }

        public void setWithWord(string withWord)
        {
            this.withWord = withWord;
        }

        public string toString()
        {
            return (this.name + " AS " + base.alias + " WITH " + this.lockingHint);
        }
    }


    


}
