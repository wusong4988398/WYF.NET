using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Exception;
using WYF.KSQL.Formater;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlInsert : SqlObject
    {
        // Fields
        public ArrayList columnList;
        private ArrayList hints;
        private string insertWord;
        private string intoWord;
        public SqlSelectBase subQuery;
        public string tableName;
        public ArrayList valueList;
        private string valuesWord;

        // Methods
        public SqlInsert(string tableName)
        {
            this.valueList = new ArrayList();
            this.columnList = new ArrayList();
            this.tableName = tableName;
        }

        public SqlInsert(string tableName, ArrayList columnList, ArrayList valueList)
        {
            this.tableName = tableName;
            this.columnList = columnList;
            this.valueList = valueList;
        }

        public SqlInsert(string tableName, int columnListSize, int valueListSize)
        {
            this.tableName = tableName;
            this.columnList = new ArrayList(columnListSize);
            this.valueList = new ArrayList(valueListSize);
        }

        public override object Clone()
        {
            int count = this.columnList.Count;
            int valueListSize = this.valueList.Count;
            SqlInsert insert = new SqlInsert(this.tableName, count, valueListSize);
            for (int i = 0; i < count; i++)
            {
                object obj2 = this.columnList[i];
                if (obj2.GetType() == typeof(SqlIdentifierExpr))
                {
                    insert.columnList.Add(((SqlIdentifierExpr)obj2).Clone());
                }
                else
                {
                    if (obj2.GetType() != typeof(string))
                    {
                        throw new IllegalStateException("unexpect expression: '" + obj2 + "'");
                    }
                    insert.columnList.Add(obj2);
                }
            }
            for (int j = 0; j < valueListSize; j++)
            {
                SqlExpr expr2 = (SqlExpr)((SqlExpr)this.valueList[j]).Clone();
                insert.valueList.Add(expr2);
            }
            insert.setInsertWord(this.getInsertWord());
            insert.setIntoWord(this.getIntoWord());
            insert.setValuesWord(this.getValuesWord());
            return insert;
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

        public string getInsertWord()
        {
            if ((this.insertWord != null) && (this.insertWord.Length != 0))
            {
                return this.insertWord;
            }
            return "INSERT";
        }

        public string getIntoWord()
        {
            if (this.intoWord == null)
            {
                return "INTO";
            }
            return this.intoWord;
        }

        public string getValuesWord()
        {
            if ((this.valuesWord != null) && (this.valuesWord.Length != 0))
            {
                return this.valuesWord;
            }
            return "VALUES";
        }

        public void output(StringBuilder buff)
        {
            try
            {
                new DrSQLFormater(buff).FormatInsertStmt(new SqlInsertStmt(this));
            }
            catch (FormaterException exception)
            {
                throw exception;
            }
        }

        public void setInsertWord(string insertWord)
        {
            this.insertWord = insertWord;
        }

        public void setIntoWord(string intoWord)
        {
            this.intoWord = intoWord;
        }

        public void setValuesWord(string valuesWord)
        {
            this.valuesWord = valuesWord;
        }

        public string toString()
        {
            StringBuilder buff = new StringBuilder();
            this.output(buff);
            return buff.ToString();
        }
    }


    



}
