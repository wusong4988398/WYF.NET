using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.exception
{
    public class ParserException: SqlTranslateException
    {
        public ParserException()
        {
        }

        public ParserException(string message):base(message)
        {
            
        }

        public ParserException(string message,  Exception e):base(message, e)
        {
            
        }

        public ParserException( string message,  int line,  int col) : base(message)
        {
            
        }

        public ParserException(Exception ex,  String ksql,  int dbType):base("format sql error. target database is '" + DbType.GetName(dbType) + "' detail message is :\n" + ex.Message + "\nsource sql is : \n" + ksql, ex)
        {
        }

        public ParserException(Exception ex, String ksql) : base("parse error. detail message is :\n" + ex.Message + "\nsource sql is : \n" + ksql, ex)
        {
        }
    }
}
