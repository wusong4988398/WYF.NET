using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.trace
{
    public  class Tracer
    {
        public static ITraceSpan Create(String type, String name)
        {
            return (ITraceSpan)TracerImpl.Create(type, name);
        }
    }
}
