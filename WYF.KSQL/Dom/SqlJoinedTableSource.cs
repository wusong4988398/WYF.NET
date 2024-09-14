using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlJoinedTableSource : SqlTableSourceBase
    {
        // Fields
        public SqlExpr condition;
        public int joinType;
        private string joinWord;
        public SqlTableSourceBase left;
        private string onWord;
        public SqlTableSourceBase right;

        // Methods
        public SqlJoinedTableSource()
        {
        }

        public SqlJoinedTableSource(SqlTableSourceBase left, SqlTableSourceBase right, int joinType, SqlExpr condition)
        {
            this.left = left;
            this.right = right;
            this.condition = condition;
            this.joinType = joinType;
            this.setOnWord("ON");
            switch (this.joinType)
            {
                case 0:
                    this.setJoinWord("INNER JOIN");
                    return;

                case 1:
                    this.setJoinWord("LEFT OUTER JOIN");
                    return;

                case 2:
                    this.setJoinWord("RIGHT OUTER JOIN");
                    return;

                case 3:
                    this.setJoinWord("FULL OUTER JOIN");
                    return;

                case 4:
                    this.setJoinWord(",");
                    return;
            }
            this.setJoinWord("");
        }

        public SqlJoinedTableSource(SqlTableSourceBase left, SqlTableSourceBase right, int joinType, SqlExpr condition, string joinWord, string onWord)
        {
            this.left = left;
            this.right = right;
            this.condition = condition;
            this.joinType = joinType;
            this.setOnWord(onWord);
            this.setJoinWord(joinWord);
        }

        public override object Clone()
        {
            SqlJoinedTableSource source = new SqlJoinedTableSource();
            if (this.left != null)
            {
                if (this.left.GetType() != typeof(SqlJoinedTableSource))
                {
                    if (this.left.GetType() != typeof(SqlTableSource))
                    {
                        throw new NotSupportedException("unexcept TableSource:" + this.left);
                    }
                    source.left = (SqlTableSource)this.left.Clone();
                }
                else
                {
                    source.left = (SqlJoinedTableSource)this.left.Clone();
                }
            }
            if (this.right != null)
            {
                if (this.right.GetType() != typeof(SqlJoinedTableSource))
                {
                    if (this.right.GetType() != typeof(SqlTableSource))
                    {
                        throw new NotSupportedException("unexcept TableSource:" + this.right);
                    }
                    source.right = (SqlTableSource)this.right.Clone();
                }
                else
                {
                    source.right = (SqlJoinedTableSource)this.right.Clone();
                }
            }
            if (this.condition != null)
            {
                source.condition = (SqlExpr)this.condition.Clone();
            }
            source.joinType = this.joinType;
            source.setOrgAlias(base.getOrgAlias());
            source.setJoinWord(this.getJoinWord());
            source.setOnWord(this.getOnWord());
            return source;
        }

        public string getJoinWord()
        {
            if (this.joinWord == null)
            {
                switch (this.joinType)
                {
                    case 0:
                        this.setJoinWord("INNER JOIN");
                        goto Label_0077;

                    case 1:
                        this.setJoinWord("LEFT OUTER JOIN");
                        goto Label_0077;

                    case 2:
                        this.setJoinWord("RIGHT OUTER JOIN");
                        goto Label_0077;

                    case 3:
                        this.setJoinWord("FULL OUTER JOIN");
                        goto Label_0077;

                    case 4:
                        this.setJoinWord(",");
                        goto Label_0077;
                }
                this.setJoinWord("");
            }
            Label_0077:
            return this.joinWord;
        }

        public string getOnWord()
        {
            if ((this.onWord != null) && (this.onWord.Trim().Length != 0))
            {
                return this.onWord;
            }
            return "ON";
        }

        public void setJoinWord(string joinWord)
        {
            this.joinWord = joinWord;
        }

        public void setOnWord(string onWord)
        {
            this.onWord = onWord;
        }
    }


   


}
