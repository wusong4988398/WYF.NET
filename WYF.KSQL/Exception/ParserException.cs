using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Exception
{
    [Serializable]
    public class ParserException : SqlTranslateException
    {
      
        private const long serialVersionUID = 1L;

    
        public ParserException()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(System.Exception ex, string ksql) : base("parse error. detail message is :\n" + ex.Message + "\nsource sql is : \n" + ksql, ex)
        {
        }

        public ParserException(string message, System.Exception e) : base(message, e)
        {
        }

        public ParserException(System.Exception ex, string ksql, int dbType) : base("format sql error. target database is '" + DatabaseType.getName(dbType) + "' detail message is :\n" + ex.Message + "\nsource sql is : \n" + ksql)
        {
        }

        public ParserException(string message, int line, int col) : base(message)
        {
        }
    }


 



}
