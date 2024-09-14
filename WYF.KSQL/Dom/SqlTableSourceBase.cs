using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public abstract class SqlTableSourceBase : SqlObject
    {
        // Fields
        public string alias;
        private string asWord;
        private string orgAlias;
        public ArrayList subQueries;

        // Methods
        public SqlTableSourceBase()
        {
            this.subQueries = new ArrayList();
            this.asWord = "";
        }

        public SqlTableSourceBase(string alias)
        {
            this.subQueries = new ArrayList();
            this.asWord = "";
            this.alias = alias;
            this.setOrgAlias(alias);
        }

        public string getAsWord()
        {
            return this.asWord;
        }

        public string getOrgAlias()
        {
            if ((this.orgAlias != null) && (this.orgAlias.Trim().Length != 0))
            {
                return this.orgAlias;
            }
            return this.alias;
        }

        public void setAlias(string alias)
        {
            this.alias = alias;
            this.setOrgAlias(alias);
        }

        public void setAsWord(string asWord)
        {
            this.asWord = asWord;
        }

        public void setOrgAlias(string orgAlias)
        {
            this.orgAlias = orgAlias;
        }
    }


    



}
