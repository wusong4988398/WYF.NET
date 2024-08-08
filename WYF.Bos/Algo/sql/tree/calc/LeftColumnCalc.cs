using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public  class LeftColumnCalc : Calc
    {
      private int _index;

    public LeftColumnCalc(int index)
    {
        this._index = index;
    }

    public object Execute(IRowFeature row1, IRowFeature row2)
    {
        return (row1 == null) ? null : row1.Get(this._index);
    }
}
}
