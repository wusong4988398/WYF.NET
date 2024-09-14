

using System;

namespace WYF.KSQL.Exception
{
    [Serializable]
    public class SqlTranslateException : System.Exception
    {
        // Methods
        public SqlTranslateException()
        {
           
        }

        public SqlTranslateException(string message) : base(message)
        {
        }

        public SqlTranslateException(string message, System.Exception e) : base(message, e)
        {
        }
    }



}
