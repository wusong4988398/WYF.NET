using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.schema
{
    public interface ITable
    {
        public string Schema { get; }
        public string Name { get; }

        public IColumn[] Columns { get; }

        IColumn LookupColumn(string name);

    }
}
