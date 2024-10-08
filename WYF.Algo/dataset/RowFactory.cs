using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataset
{
    public class RowFactory
    {
        internal static IRow CreateRow(RowMeta rowMeta, IDataReader dataReader)
        {
            return new ResultSetRowImpl(rowMeta, dataReader);
        }
    }
}
