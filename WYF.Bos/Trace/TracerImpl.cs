using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.trace
{
    public class TracerImpl
    {
        public static TraceSpanImpl Create(string type, string name)
        {
            //if (TraceConfig.isTraceEnable() && TraceConfig.isTypeEnable(type) && TraceConfig.isSpanEnable(type, name))
            //    return new TraceSpanImpl(type, name);
            return TraceSpanImpl.CreateEmpty(type, name);
        }
    }
}
