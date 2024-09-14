using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlDelete : SqlObject
    {
       
        public SqlExpr condition;
        private string deleteWord;
        private string fromWord;
        private ArrayList hints;
        public string tableName;
        public SqlTableSourceBase tableSource;
        private string whereWord;

 
        public SqlDelete()
        {
        }

        public SqlDelete(string tableName)
        {
            this.tableName = tableName;
        }

        public override object Clone()
        {
            SqlDelete delete = new SqlDelete
            {
                tableName = this.tableName
            };
            SqlTableSourceBase tableSource = this.tableSource;
            if (this.condition != null)
            {
                delete.condition = (SqlExpr)this.condition.Clone();
            }
            delete.setDeleteWord(this.deleteWord);
            delete.setFromWord(this.fromWord);
            delete.setWhereWord(this.whereWord);
            return delete;
        }

        public string getDeleteWord()
        {
            if ((this.deleteWord != null) && (this.deleteWord.Length != 0))
            {
                return this.deleteWord;
            }
            return "DELETE";
        }

        public string getFromWord()
        {
            if ((this.fromWord != null) && (this.fromWord.Length != 0))
            {
                return this.fromWord;
            }
            return "FROM";
        }

        public ArrayList getHints()
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

        public string getWhereWord()
        {
            if ((this.whereWord != null) && (this.whereWord.Length != 0))
            {
                return this.whereWord;
            }
            return "WHERE";
        }

        public override void output(StringBuilder buffer, string prefix)
        {
            if (prefix != null)
            {
                buffer.Append(prefix);
            }
            buffer.Append("DELETE ");
            if ((this.tableName != null) && (this.tableName.Length != 0))
            {
                if (this.tableSource != null)
                {
                    throw new IllegalStateException("not support");
                }
                buffer.Append("FROM ");
                buffer.Append(this.tableName);
            }
            else
            {
                buffer.Append("FROM ");
                this.tableSource.output(buffer, null);
            }
            if (this.condition != null)
            {
                buffer.Append(" WHERE ");
                this.condition.output(buffer);
            }
        }

        public void setDeleteWord(string deleteWord)
        {
            this.deleteWord = deleteWord;
        }

        public void setFromWord(string fromWord)
        {
            this.fromWord = fromWord;
        }

        public void setWhereWord(string whereWord)
        {
            this.whereWord = whereWord;
        }

        public string toString()
        {
            StringBuilder buffer = new StringBuilder();
            this.output(buffer, null);
            return buffer.ToString();
        }
    }






}
