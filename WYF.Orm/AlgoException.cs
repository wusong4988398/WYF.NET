using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.SqlParser
{
    public class AlgoException : Exception
    {
        public AlgoException(string message) : base(message)
        {

        }

        public AlgoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
