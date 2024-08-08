
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.trace
{
    public interface ITraceSpan
    {
        public string Type { get; }

        public string Name { get; }
        public int Cost { get; }

        ITraceSpan AddTag(string key, string value, bool force);

    }
}
