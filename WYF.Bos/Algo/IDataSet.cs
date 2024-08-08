using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo
{
    public interface IDataSet : IDisposable, IEnumerable<IRow>, IEnumerator<IRow>
    {
        IDataSet Where(string expr);

        IDataSet Where(string expr, Dictionary<string, object> paramter);

        DataSet Filter(string expr);
    }
}
