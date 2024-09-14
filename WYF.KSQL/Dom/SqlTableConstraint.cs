using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public abstract class SqlTableConstraint : SqlObject
    {
        // Fields
        private string constraintWord;
        public string name;

        // Methods
        public SqlTableConstraint()
        {
        }

        public SqlTableConstraint(string name)
        {
            this.name = name;
            this.constraintWord = "CONSTRAINT";
        }

        public string getConstraintWord()
        {
            if ((this.constraintWord != null) && (this.constraintWord.Trim().Length != 0))
            {
                return this.constraintWord;
            }
            return "CONSTRAINT";
        }

        public void setConstraintWord(string constraintWord)
        {
            this.constraintWord = constraintWord;
        }
    }


 



}
