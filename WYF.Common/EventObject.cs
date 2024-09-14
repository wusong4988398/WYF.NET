using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    [Serializable]
    public class EventObject
    {
        [NonSerialized]
        protected Object source;

        public EventObject(Object source)
        {
            if (source == null)
                throw new ArgumentNullException("null source");

            this.source = source;
        }

        public Object GetSource()
        {
            return source;
        }

        public String ToString()
        {

            return this.GetType().Name + "[source=" + source + "]";
        }
    }
}
