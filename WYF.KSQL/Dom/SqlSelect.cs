using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlSelect : SqlSelectBase
    {
        // Fields
        public SqlExpr condition;
        public int distinct;
        private string distinctWord;
        private string fromWord;
        public ArrayList groupBy;
        private string groupByWord;
        public bool hasWithRollUp;
        public SqlExpr having;
        private string havingWord;
        public HierarchicalQueryClause hierarchicalQueryClause;
        public SqlSelectInto into;
        public SqlSelectLimit limit;
        public ArrayList selectList;
        public SqlTableSourceBase tableSource;
        private string topWord;
        private string whereWord;
        private string withRollUpWord;

        // Methods
        public SqlSelect()
        {
            this.groupBy = new ArrayList();
            this.selectList = new ArrayList();
        }

        public SqlSelect(int selectListSize)
        {
            this.groupBy = new ArrayList();
            this.selectList = new ArrayList(selectListSize);
        }

        public override object Clone()
        {
            int count = this.selectList.Count;
            SqlSelect select = new SqlSelect(count);
            for (int i = 0; i < count; i++)
            {
                SqlSelectItem item = (SqlSelectItem)((SqlSelectItem)this.selectList[i]).Clone();
                select.selectList.Add(item);
            }
            select.getHints().AddRange(base.getHints());
            if (this.tableSource != null)
            {
                select.tableSource = (SqlTableSourceBase)this.tableSource.Clone();
            }
            select.distinct = this.distinct;
            if (this.limit != null)
            {
                select.limit = (SqlSelectLimit)this.limit.Clone();
            }
            if (this.condition != null)
            {
                select.condition = (SqlExpr)this.condition.Clone();
            }
            if (this.hierarchicalQueryClause != null)
            {
                select.hierarchicalQueryClause = (HierarchicalQueryClause)this.hierarchicalQueryClause.Clone();
            }
            if (this.groupBy != null)
            {
                int num3 = this.groupBy.Count;
                for (int j = 0; j < num3; j++)
                {
                    SqlExpr expr = (SqlExpr)((SqlExpr)this.groupBy[j]).Clone();
                    select.groupBy.Add(expr);
                }
            }
            if (this.having != null)
            {
                select.having = (SqlExpr)this.having.Clone();
            }
            if (this.into != null)
            {
                select.into = (SqlSelectInto)this.into.Clone();
            }
            if (base.orderBy != null)
            {
                for (int k = 0; k < base.orderBy.Count; k++)
                {
                    select.orderBy.Add(((SqlOrderByItem)base.orderBy[k]).Clone());
                }
            }
            select.hasWithRollUp = this.hasWithRollUp;
            select.setFromWord(this.fromWord);
            select.setWhereWord(this.whereWord);
            select.setSelectWord(base.getSelectWord());
            select.setGroupByWord(this.groupByWord);
            select.setHavingWord(this.havingWord);
            select.setOrderByWord(base.getOrderByWord());
            select.setTopWord(this.topWord);
            select.setWithRollUpWord(this.withRollUpWord);
            return select;
        }

        public string getDistinctWord()
        {
            if ((this.distinctWord != null) && (this.distinctWord.Trim().Length != 0))
            {
                return this.distinctWord;
            }
            return "Distinct";
        }

        public string getFromWord()
        {
            if ((this.fromWord != null) && (this.fromWord.Trim().Length != 0))
            {
                return this.fromWord;
            }
            return "FROM";
        }

        public string getGroupByWord()
        {
            if ((this.groupByWord != null) && (this.groupByWord.Trim().Length != 0))
            {
                return this.groupByWord;
            }
            return "GROUP BY";
        }

        public string getHavingWord()
        {
            if ((this.havingWord != null) && (this.havingWord.Trim().Length != 0))
            {
                return this.havingWord;
            }
            return "HAVING";
        }

        public string getTopWord()
        {
            if ((this.topWord != null) && (this.topWord.Trim().Length != 0))
            {
                return this.topWord;
            }
            return "TOP";
        }

        public string getWhereWord()
        {
            if ((this.whereWord != null) && (this.whereWord.Trim().Length != 0))
            {
                return this.whereWord;
            }
            return "WHERE";
        }

        public string getWithRollUpWord()
        {
            if ((this.withRollUpWord != null) && (this.withRollUpWord.Trim().Length != 0))
            {
                return this.withRollUpWord;
            }
            return "WITH ROLLUP";
        }

        public void setDistinctWord(string distinctWord)
        {
            this.distinctWord = distinctWord;
        }

        public void setFromWord(string fromWord)
        {
            this.fromWord = fromWord;
        }

        public void setGroupByWord(string groupByWord)
        {
            this.groupByWord = groupByWord;
        }

        public void setHavingWord(string havingWord)
        {
            this.havingWord = havingWord;
        }

        public void setTopWord(string topWord)
        {
            this.topWord = topWord;
        }

        public void setWhereWord(string whereWord)
        {
            this.whereWord = whereWord;
        }

        public void setWithRollUpWord(string withRollUpWord)
        {
            this.withRollUpWord = withRollUpWord;
        }
    }


  


}
