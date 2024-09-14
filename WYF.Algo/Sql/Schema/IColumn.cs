using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Schema
{
    public interface IColumn
    {
        ISchema Schema { get; }

        ITable Table { get; }

        int Index { get; }

        string Name { get; }

        string FullName { get; }

        DataType DataType { get; }
    }
}
