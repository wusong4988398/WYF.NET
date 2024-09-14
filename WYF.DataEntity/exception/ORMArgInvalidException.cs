using WYF.Bos.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity
{
    [Serializable]
    public class ORMArgInvalidException : OrmException
    {
    
        protected ORMArgInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ORMArgInvalidException(string code, string message) : base(code, message)
        {
        }

        public ORMArgInvalidException(string code, string message, Exception inner) : base(code, message, inner)
        {
        }
    }
}
