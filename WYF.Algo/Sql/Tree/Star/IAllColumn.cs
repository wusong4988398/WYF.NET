using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.bind;

namespace WYF.Algo.Sql.Tree.Star
{

    public interface IAllColumn
    {
        ColumnRef[] GetAll();
    }

}
