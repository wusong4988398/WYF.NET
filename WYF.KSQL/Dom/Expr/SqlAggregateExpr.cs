using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlAggregateExpr : SqlExpr
    {
        // Fields
        public string methodName;
        public int option;
        private string optionWord;
        public SqlOverExpr overExpr;
        public ArrayList paramList;

        // Methods
        public SqlAggregateExpr(string methodName) : base(11)
        {
            this.paramList = new ArrayList();
            this.overExpr = new SqlOverExpr();
            this.methodName = methodName;
            this.option = 1;
            this.setExprWord(methodName);
            this.setOptionWord("ALL");
        }

        public SqlAggregateExpr(string methodName, int option) : base(11)
        {
            this.paramList = new ArrayList();
            this.overExpr = new SqlOverExpr();
            this.methodName = methodName;
            this.option = option;
            this.setExprWord(methodName);
            if (this.option == 1)
            {
                this.setOptionWord("ALL");
            }
            else
            {
                this.setOptionWord("DISTINCT");
            }
        }

        public SqlAggregateExpr(string methodName, int option, string optionWord) : base(11)
        {
            this.paramList = new ArrayList();
            this.overExpr = new SqlOverExpr();
            this.methodName = methodName;
            this.option = option;
            this.setExprWord(methodName);
            this.setOptionWord(optionWord);
        }

        public override object Clone()
        {
            SqlAggregateExpr expr = new SqlAggregateExpr(this.methodName)
            {
                option = this.option
            };
            if (this.paramList != null)
            {
                int num = 0;
                int count = this.paramList.Count;
                while (num < count)
                {
                    SqlExpr expr2 = (SqlExpr)((SqlExpr)this.paramList[num]).Clone();
                    expr.paramList.Add(expr2);
                    num++;
                }
            }
            expr.setExprWord(this.getExprWord());
            expr.setOptionWord(this.getOptionWord());
            return expr;
        }

        public string getOptionWord()
        {
            return this.optionWord;
        }

        public bool HasOver()
        {
            return ((this.overExpr.orderBy.Count + this.overExpr.partition.Count) > 0);
        }

        public void setOptionWord(string optionWord)
        {
            this.optionWord = optionWord;
        }
    }


   



}
