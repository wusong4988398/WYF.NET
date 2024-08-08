using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.schema
{
    public interface IColumn
    {
        public string Schema { get; }
        public ITable Table { get; }

        public int Index { get; }

        public string Name { get; }

        public string FullName { get; }

        public DataType DataType { get; }

    }
}
