using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlTableCheck : SqlTableConstraint
    {
        // Fields
        private string checkWord;
        public SqlExpr expr;

        // Methods
        public SqlTableCheck()
        {
        }

        public SqlTableCheck(string name) : base(name)
        {
        }

        public SqlTableCheck(string name, SqlExpr expr) : base(name)
        {
            this.expr = expr;
        }

        public override object Clone()
        {
            SqlTableCheck check = null;
            if (this.expr != null)
            {
                check = new SqlTableCheck(base.name, (SqlExpr)this.expr.Clone());
            }
            else
            {
                check = new SqlTableCheck(base.name);
            }
            check.setCheckWord(this.getCheckWord());
            check.setConstraintWord(base.getConstraintWord());
            return check;
        }

        public string getCheckWord()
        {
            return this.checkWord;
        }

        public void setCheckWord(string checkWord)
        {
            this.checkWord = checkWord;
        }
    }


   



}
