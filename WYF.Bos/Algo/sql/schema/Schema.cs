using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.schema
{
    public interface ISchema
    {
        public string Name { get; }
        public ITable[] Tables { get; }
        ITable GetTable(string name);
        IColumn GetColumn(string[] nameParts);
        public IFuncFactory FuncFactory { get; }
        public bool IgnoreCase { get; }

    }
}
