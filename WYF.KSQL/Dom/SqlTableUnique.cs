using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlTableUnique : SqlTableConstraint
    {
        // Fields
        public bool clustered;
        private string clusteredWord;
        public ArrayList columnList;
        private string uniqueWord;

        // Methods
        public SqlTableUnique()
        {
            this.columnList = new ArrayList();
        }

        public SqlTableUnique(string name) : base(name)
        {
            this.columnList = new ArrayList();
            this.setUniqueWord("UNIQUE");
            this.setClusteredWord("");
        }

        public override object Clone()
        {
            SqlTableUnique unique = new SqlTableUnique(base.name);
            if (this.columnList != null)
            {
                int num = 0;
                int count = this.columnList.Count;
                while (num < count)
                {
                    string str = (string)this.columnList[num];
                    unique.columnList.Add(str);
                    num++;
                }
            }
            unique.clustered = this.clustered;
            unique.setConstraintWord(base.getConstraintWord());
            unique.setUniqueWord(this.getUniqueWord());
            unique.setClusteredWord(this.getClusteredWord());
            return unique;
        }

        public string getClusteredWord()
        {
            return this.clusteredWord;
        }

        public string getUniqueWord()
        {
            return this.uniqueWord;
        }

        public void setClusteredWord(string clusteredWord)
        {
            this.clusteredWord = clusteredWord;
        }

        public void setUniqueWord(string uniqueWord)
        {
            this.uniqueWord = uniqueWord;
        }
    }






}
