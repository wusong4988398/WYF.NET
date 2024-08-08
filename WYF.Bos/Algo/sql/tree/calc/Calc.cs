using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public interface Calc
    {
        Object Execute(IRowFeature row1, IRowFeature row2);
    }
}
