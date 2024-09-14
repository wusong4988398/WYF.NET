using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Util
{
    public class UUTNException : RuntimeException
    {
        // Methods
        public UUTNException()
        {
        }

        public UUTNException(System.Exception arg0)
        {
        }

        public UUTNException(string arg0) : base(arg0)
        {
        }

        public UUTNException(string arg0, System.Exception arg1) : base(arg0, arg1)
        {
        }
    }


 


}
