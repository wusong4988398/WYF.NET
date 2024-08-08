using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public  class RightColumnCalc : Calc
    {
      private int _index;

    public RightColumnCalc(int index)
    {
        this._index = index;
    }

    public Object Execute(IRowFeature row1, IRowFeature row2)
    {
        return (row2 == null) ? null : row2.Get(this._index);
    }
}
}
