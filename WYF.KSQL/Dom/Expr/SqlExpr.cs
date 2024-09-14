using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Exception;
using WYF.KSQL.Formater;
using WYF.KSQL.Parser;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public abstract class SqlExpr : SqlObject
    {
    
        private ArrayList _subQueries;
        private string exprWord = "";
        public int type;

     
        protected SqlExpr(int type)
        {
            this.type = type;
            if (type == 13)
            {
                this.setExprWord("IN");
            }
        }

        public override object Clone()
        {
            object obj2;
            string text = this.ToString();
            try
            {
                obj2 = new SqlExprParser(text).expr();
            }
            catch (ParserException exception)
            {
                throw new RuntimeException(exception.Message, exception);
            }
            return obj2;
        }

        public virtual string getExprWord()
        {
            return this.exprWord;
        }

        public virtual string getOrgValue()
        {
            return this.toString();
        }

        public virtual void output(StringBuilder buff)
        {
            try
            {
                new DrSQLFormater(buff).FormatExpr(this);
            }
            catch (FormaterException exception)
            {
                throw new RuntimeException(exception.Message, exception);
            }
        }

        public override void output(StringBuilder buffer, string prefix)
        {
            buffer.Append(prefix);
            buffer.Append("SqlExpr:");
            buffer.Append(this.typename());
            buffer.Append("\n");
        }

        public virtual void setExprWord(string exprWord)
        {
            this.exprWord = exprWord;
        }

        public ArrayList subQueries()
        {
            if (this._subQueries == null)
            {
                lock (this)
                {
                    if (this._subQueries == null)
                    {
                        this._subQueries = new ArrayList();
                    }
                }
            }
            return this._subQueries;
        }

        public static SqlExpr toCharExpr(string value)
        {
            if (value == null)
            {
                return SqlNullExpr.instance;
            }
            return new SqlCharExpr(value);
        }

        public static SqlExpr toDateExpr(string value)
        {
            DateTime time = Convert.ToDateTime(value);
            SqlDateTimeExpr expr = new SqlDateTimeExpr(time.Year, time.Month, time.Day);
            expr.setTimeType(-19001);
            return expr;
        }

        public static SqlExpr toDateTimeExpr(string value)
        {
            DateTime time = Convert.ToDateTime(value);
            return new SqlDateTimeExpr(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public static SqlExpr toExpr(decimal value)
        {
            return new SqlDoubleExpr(value.ToString());
        }

        public static SqlExpr toExpr(double value)
        {
            return new SqlDoubleExpr(value.ToString());
        }

        public static SqlExpr toExpr(int value)
        {
            return new SqlIntExpr(value.ToString());
        }

        public static SqlExpr toExpr(long value)
        {
            return new SqlIntExpr(value.ToString());
        }

        public static SqlExpr toExpr(byte[] value)
        {
            if (value != null)
            {
                throw new System.Exception("The method or operation is not implemented.");
            }
            return SqlNullExpr.instance;
        }

        public static SqlExpr toNCharExpr(string value)
        {
            if (value == null)
            {
                return SqlNullExpr.instance;
            }
            return new SqlNCharExpr(value);
        }

        public virtual string toString()
        {
            StringBuilder buff = new StringBuilder();
            this.output(buff);
            return buff.ToString();
        }

        public string ToStringNotTabFuncFormat()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                new DrSQLFormater(sb) { IsTabFuncFormat = false }.FormatExpr(this);
            }
            catch (FormaterException exception)
            {
                throw new RuntimeException(exception.Message, exception);
            }
            return sb.ToString();
        }

        public static SqlExpr toXmlExpr(string value)
        {
            if (value == null)
            {
                return SqlNullExpr.instance;
            }
            return new SqlXmlExpr(value);
        }

        public string typename()
        {
            return ExprType.typename(this.type);
        }
    }


 



}
