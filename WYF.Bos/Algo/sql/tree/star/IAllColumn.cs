using WYF.Bos.algo.sql.tree.bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.star
{
    public interface IAllColumn
    {
        ColumnRef[] GetAll();
    }
}
