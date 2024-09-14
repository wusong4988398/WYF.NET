using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Exception
{
    [Serializable]
    public class IllegalStateException : System.Exception
    {
        // Methods
        public IllegalStateException()
        {
        }

        public IllegalStateException(string message) : base(message)
        {
        }

        public IllegalStateException(string message, System.Exception e) : base(message, e)
        {
        }
    }


 



}
