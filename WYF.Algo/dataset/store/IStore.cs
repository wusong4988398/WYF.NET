using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.SQLite.PythonSQLite;

namespace WYF.Algo.dataset.store
{
    public interface IStore : IDisposable
    {
        // 获取行迭代器
        IEnumerator<IRow> GetRowIterator();

        // 写入行数据
        void Write(IEnumerable<IRow> rows);

        // 写入单个行数据
        void Write(IRow row);

        // 创建存储的副本
        IStore Copy();

        // 获取行元数据
        RowMeta GetRowMeta();

        // 获取存储中行的数量
        int Size { get; }
    }
}

