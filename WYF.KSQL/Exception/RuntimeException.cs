using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Exception
{
    [Serializable]
    public class RuntimeException : SqlTranslateException
    {
        // Methods
        public RuntimeException()
        {
        }

        public RuntimeException(string message) : base(message)
        {
        }

        public RuntimeException(string message, System.Exception e) : base(message, e)
        {
        }
    }


 



}
