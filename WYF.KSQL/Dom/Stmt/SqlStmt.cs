using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Exception;
using WYF.KSQL.Formater;
using WYF.KSQL.Parser;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public abstract class SqlStmt : SqlObject, ICloneable
    {
        // Fields
        public int type;

        // Methods
        protected SqlStmt(int type)
        {
            this.type = type;
        }

        public override object Clone()
        {
            object obj2;
            string sql = this.toString();
            try
            {
                obj2 = new SqlStmtParser(sql).stmt();
            }
            catch (ParserException exception)
            {
                throw new RuntimeException(exception.Message, exception);
            }
            return obj2;
        }

        public void output(StringBuilder buff)
        {
            this.output(buff, null);
        }

        public override void output(StringBuilder buff, string prefix)
        {
            try
            {
                if (prefix != null)
                {
                    buff.Append(prefix);
                }
                new DrSQLFormater(buff).FormatStmt(this);
            }
            catch (FormaterException exception)
            {
                throw new RuntimeException(exception.Message, exception);
            }
        }

        public string toString()
        {
            StringBuilder buff = new StringBuilder();
            this.output(buff);
            return buff.ToString();
        }

        public string typename()
        {
            return StmtType.typename(this.type);
        }
    }





}
