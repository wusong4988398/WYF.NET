using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlIfStmt : SqlStmt
    {
        // Fields
        private string beginWord;
        public SqlExpr condition;
        private string elseBeginWord;
        private string elseEndWord;
        private string elseWord;
        private string endWord;
        public IList falseStmtList;
        private string ifWord;
        public IList trueStmtList;

        // Methods
        public SqlIfStmt() : base(9)
        {
            this.trueStmtList = new ArrayList();
            this.falseStmtList = new ArrayList();
        }

        public SqlIfStmt(SqlExpr condition) : base(9)
        {
            this.trueStmtList = new ArrayList();
            this.falseStmtList = new ArrayList();
            this.condition = condition;
        }

        public string getBeginWord()
        {
            if ((this.beginWord != null) && (this.beginWord.Length != 0))
            {
                return this.beginWord;
            }
            return "";
        }

        public string getElseBeginWord()
        {
            if ((this.elseBeginWord != null) && (this.elseBeginWord.Length != 0))
            {
                return this.elseBeginWord;
            }
            return "";
        }

        public string getElseEndWord()
        {
            if ((this.elseEndWord != null) && (this.elseEndWord.Length != 0))
            {
                return this.elseEndWord;
            }
            return "";
        }

        public string getElseWord()
        {
            if ((this.elseWord != null) && (this.elseWord.Length != 0))
            {
                return this.elseWord;
            }
            return "ELSE";
        }

        public string getEndWord()
        {
            if ((this.endWord != null) && (this.endWord.Length != 0))
            {
                return this.endWord;
            }
            return "";
        }

        public string getIfWord()
        {
            if ((this.ifWord != null) && (this.ifWord.Length != 0))
            {
                return this.ifWord;
            }
            return "IF";
        }

        public void setBeginWord(string beginWord)
        {
            this.beginWord = beginWord;
        }

        public void setElseBeginWord(string elseBeginWord)
        {
            this.elseBeginWord = elseBeginWord;
        }

        public void setElseEndWord(string elseEndWord)
        {
            this.elseEndWord = elseEndWord;
        }

        public void setElseWord(string elseWord)
        {
            this.elseWord = elseWord;
        }

        public void setEndWord(string endWord)
        {
            this.endWord = endWord;
        }

        public void setIfWord(string ifWord)
        {
            this.ifWord = ifWord;
        }
    }


   



}
