using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Schema
{
    public interface ISchema
    {
        string Name { get; }

        ITable[] Tables { get; }

        ITable this[string tableName] { get; }

        IColumn this[string[] columnPath] { get; }

        IFuncFactory FuncFactory { get; }

        bool IsIgnoreCase { get; }
    }
}
