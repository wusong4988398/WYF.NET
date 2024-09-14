using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlTablePrimaryKey : SqlTableConstraint
    {
        // Fields
        public bool clustered;
        private string clusteredWord;
        public ArrayList columnList;
        private string primaryKeyWord;

        // Methods
        public SqlTablePrimaryKey()
        {
            this.columnList = new ArrayList();
            this.clustered = true;
        }

        public SqlTablePrimaryKey(string name) : base(name)
        {
            this.columnList = new ArrayList();
            this.clustered = true;
            this.setPrimaryKeyWord("PRIMARY KEY");
            this.setClusteredWord("");
        }

        public override object Clone()
        {
            SqlTablePrimaryKey key = new SqlTablePrimaryKey(base.name);
            if (this.columnList != null)
            {
                int num = 0;
                int count = this.columnList.Count;
                while (num < count)
                {
                    string str = (string)this.columnList[num];
                    key.columnList.Add(str);
                    num++;
                }
            }
            key.clustered = this.clustered;
            key.setConstraintWord(base.getConstraintWord());
            key.setPrimaryKeyWord(this.getPrimaryKeyWord());
            key.setClusteredWord(this.getClusteredWord());
            return key;
        }

        public string getClusteredWord()
        {
            return this.clusteredWord;
        }

        public string getPrimaryKeyWord()
        {
            return this.primaryKeyWord;
        }

        public void setClusteredWord(string clusteredWord)
        {
            this.clusteredWord = clusteredWord;
        }

        public void setPrimaryKeyWord(string primaryKeyWord)
        {
            this.primaryKeyWord = primaryKeyWord;
        }
    }





}
