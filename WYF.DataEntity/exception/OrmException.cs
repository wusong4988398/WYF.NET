using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity
{
    [Serializable]
    public class OrmException : ApplicationException

    {

        private string mCode;


        protected OrmException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.mCode = info.GetString("Code");
        }

        public OrmException(string code, string message) : base(message)
        {
            this.mCode = code;
        }

        public OrmException(string code, string message, Exception inner) : base(message, inner)
        {
            this.mCode = code;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", this.mCode, typeof(string));
        }


        public string Code
        {
            get
            {
                return this.mCode;
            }

        }
    }
}
