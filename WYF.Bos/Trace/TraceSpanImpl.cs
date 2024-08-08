using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.trace
{
    public class TraceSpanImpl : ITraceSpan
    {
        private string _type;

        private string _name;
        public string Type => this._type;

        public string Name => this._name;

        public int Cost => throw new NotImplementedException();

        public ITraceSpan AddTag(string key, string value, bool force)
        {
            throw new NotImplementedException();
        }

        public static TraceSpanImpl CreateEmpty(string type, string name)
        {
            TraceSpanImpl span = new TraceSpanImpl();
            span._type = type;
            span._name = name;
            return span;
        }
    }
}
