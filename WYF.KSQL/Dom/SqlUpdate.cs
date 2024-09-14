using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlUpdate : SqlObject
    {
        // Fields
        private string asWord;
        public SqlExpr condition;
        private string fromWord;
        private ArrayList hints;
        private string setWord;
        public SqlTableSourceBase tableSource;
        public ArrayList updateList;
        public SqlTableSource updateTable;
        private string updateWord;
        private string whereWord;

        // Methods
        public SqlUpdate()
        {
            this.updateList = new ArrayList();
        }

        public SqlUpdate(int updateListSize)
        {
            this.updateList = new ArrayList(updateListSize);
        }

        public override object Clone()
        {
            int count = this.updateList.Count;
            SqlUpdate update = new SqlUpdate(count);
            if (this.updateTable != null)
            {
                update.updateTable = (SqlTableSource)this.updateTable.Clone();
            }
            if (this.tableSource != null)
            {
                update.tableSource = (SqlTableSourceBase)this.tableSource.Clone();
            }
            if (this.updateList != null)
            {
                for (int i = 0; i < count; i++)
                {
                    AbstractUpdateItem item = (AbstractUpdateItem)((AbstractUpdateItem)this.updateList[i]).Clone();
                    this.updateList.Add(item);
                }
            }
            update.setFromWord(this.fromWord);
            update.setSetWord(this.setWord);
            update.setUpdateWord(this.updateWord);
            update.setWhereWord(this.whereWord);
            update.setAsWord(this.asWord);
            return update;
        }

        public string getAsWord()
        {
            if ((this.asWord != null) && (this.asWord.Length != 0))
            {
                return this.asWord;
            }
            return "AS";
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

        public string getSetWord()
        {
            if ((this.setWord != null) && (this.setWord.Length != 0))
            {
                return this.setWord;
            }
            return "SET";
        }

        public string getUpdateWord()
        {
            if ((this.updateWord != null) && (this.updateWord.Length != 0))
            {
                return this.updateWord;
            }
            return "UPDATE";
        }

        public string getWhereWord()
        {
            if ((this.whereWord != null) && (this.whereWord.Length != 0))
            {
                return this.whereWord;
            }
            return "WHERE";
        }

        public void setAsWord(string asWord)
        {
            this.asWord = asWord;
        }

        public void setFromWord(string fromWord)
        {
            this.fromWord = fromWord;
        }

        public void setSetWord(string setWord)
        {
            this.setWord = setWord;
        }

        public void setUpdateWord(string updateWord)
        {
            this.updateWord = updateWord;
        }

        public void setWhereWord(string whereWord)
        {
            this.whereWord = whereWord;
        }
    }


 



}
