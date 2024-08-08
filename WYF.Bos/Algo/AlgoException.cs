using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo
{


    public class AlgoException : Exception
    {
        public AlgoException() : base() { }

        public AlgoException(string message) : base(message) { }

        public AlgoException(string message, Exception innerException) : base(message, innerException) { }

        public AlgoException(string message, params object[] args) : base(String.Format(message, args)) { }

        public static AlgoException Create(string message, params object[] args)
        {
            return new AlgoException(String.Format(message, args));
        }

        public static AlgoException Create(string message, Exception innerException, params object[] args)
        {
            return new AlgoException(String.Format(message, args), innerException);
        }

        public static AlgoException Wrap(Exception cause)
        {
            if (cause is AlgoException)
                throw cause as AlgoException;
            throw new AlgoException(cause.Message, cause);
        }
    }
}
