using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Formater
{
    public class FormaterException : SqlTranslateException
    {
        // Methods
        public FormaterException()
        {
        }

        public FormaterException(string message) : base(message)
        {
        }

        public FormaterException(string message, System.Exception e) : base(message, e)
        {
        }
    }





}
