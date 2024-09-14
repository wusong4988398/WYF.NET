using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class HierarchicalQueryClause : SqlObject
    {
        // Fields
        public SqlExpr connectByCondition;
        private string connectByWord;
        private const long serialVersionUID = 1L;
        public SqlExpr startWithCondition;
        private string startWithWord;

        // Methods
        public override object Clone()
        {
            HierarchicalQueryClause clause = new HierarchicalQueryClause();
            if (this.startWithCondition != null)
            {
                clause.startWithCondition = (SqlExpr)this.startWithCondition.Clone();
            }
            if (this.connectByCondition != null)
            {
                clause.connectByCondition = (SqlExpr)this.connectByCondition.Clone();
            }
            return clause;
        }

        public string getConnectByWord()
        {
            if ((this.connectByWord != null) && (this.connectByWord.Length != 0))
            {
                return this.connectByWord;
            }
            return "CONNECT BY";
        }

        public string getStartWithWord()
        {
            if ((this.startWithWord != null) && (this.startWithWord.Length != 0))
            {
                return this.startWithWord;
            }
            return "START WITH";
        }

        public override void output(StringBuilder buffer, string prefix)
        {
            if (prefix != null)
            {
                buffer.Append(prefix);
            }
            if (this.startWithCondition != null)
            {
                buffer.Append(" ");
                buffer.Append(this.getStartWithWord());
                buffer.Append(" ");
                this.startWithCondition.output(buffer);
            }
            if (this.connectByCondition == null)
            {
                throw new IllegalStateException("connectByCondition is null");
            }
            buffer.Append(" ");
            buffer.Append(this.getConnectByWord());
            buffer.Append(" ");
            this.connectByCondition.output(buffer);
        }

        public void setConnectByWord(string connectByWord)
        {
            this.connectByWord = connectByWord;
        }

        public void setStartWithWord(string startWithWord)
        {
            this.startWithWord = startWithWord;
        }

        public string toString()
        {
            StringBuilder buffer = new StringBuilder();
            this.output(buffer, null);
            return buffer.ToString();
        }
    }


   


}
