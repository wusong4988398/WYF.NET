using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlMerge : SqlObject
    {
        // Fields
        private ArrayList hints;
        private string intoWord;
        public SqlMergeMatched MatchedSql;
        private string mergeWord;
        public SqlMergeNotMatched NotMatchedSql;
        public SqlExpr OnExpr;
        private string onWord;
        public SqlExpr SetExpr;
        private string setWord;
        public SqlTableSource UpdateTable;
        private string updateWord;
        public SqlExpr UsingExpr;
        public SqlTableSource UsingTable;
        public string UsingTableAlias;
        private string usingWord;
        private string whenWord;

        // Methods
        public override object Clone()
        {
            SqlMerge merge = new SqlMerge();
            merge.SetUsingWord(this.usingWord);
            merge.SetOnWord(this.onWord);
            merge.SetWhenWord(this.whenWord);
            merge.SetUpdateWord(this.updateWord);
            merge.SetSetWord(this.setWord);
            return merge;
        }

        public ArrayList GetHints()
        {
            if (this.hints == null)
            {
                lock (this)
                {
                    if (this.hints == null)
                    {
                        this.hints = new ArrayList();
                    }
                }
            }
            return this.hints;
        }

        public string GetIntoWord()
        {
            if ((this.intoWord != null) && (this.intoWord.Length != 0))
            {
                return this.intoWord;
            }
            return "INTO";
        }

        public string GetMergeWord()
        {
            if ((this.mergeWord != null) && (this.mergeWord.Length != 0))
            {
                return this.mergeWord;
            }
            return "MERGE";
        }

        public string GetOnWord()
        {
            if ((this.onWord != null) && (this.onWord.Length != 0))
            {
                return this.onWord;
            }
            return "ON";
        }

        public string GetSetWord()
        {
            if ((this.setWord != null) && (this.setWord.Length != 0))
            {
                return this.setWord;
            }
            return "SET";
        }

        public string GetUpdateWord()
        {
            if ((this.updateWord != null) && (this.updateWord.Length != 0))
            {
                return this.updateWord;
            }
            return "UPDATE";
        }

        public string GetUsingWord()
        {
            if ((this.usingWord != null) && (this.usingWord.Length != 0))
            {
                return this.usingWord;
            }
            return "USING";
        }

        public string GetWhenWord()
        {
            if ((this.whenWord != null) && (this.whenWord.Length != 0))
            {
                return this.whenWord;
            }
            return "WHEN";
        }

        public void SetIntoWord(string intoWord)
        {
            this.intoWord = intoWord;
        }

        public void SetMergeWord(string mergeWord)
        {
            this.mergeWord = mergeWord;
        }

        public void SetOnWord(string onWord)
        {
            this.onWord = onWord;
        }

        public void SetSetWord(string setWord)
        {
            this.setWord = setWord;
        }

        public void SetUpdateWord(string updateWord)
        {
            this.updateWord = updateWord;
        }

        public void SetUsingWord(string usingWord)
        {
            this.usingWord = usingWord;
        }

        public void SetWhenWord(string whenWord)
        {
            this.whenWord = whenWord;
        }
    }





}
