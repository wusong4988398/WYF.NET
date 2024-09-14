using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Schema
{
    public interface ITable
    {
        ISchema Schema { get; }

        string Name { get; }

        IColumn[] Columns { get; }

        IColumn this[string columnName] { get; }
    }
}
