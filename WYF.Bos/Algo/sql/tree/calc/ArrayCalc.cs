using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
   public  class ArrayCalc : Calc
    {
  private Calc[] children;

    public ArrayCalc(Calc[] children)
    {
        this.children = children;
    }

    public Object Execute(IRowFeature row1, IRowFeature row2)
    {
        Object[] values = new Object[this.children.Length];
        for (int i = 0; i < values.Length; i++)
            values[i] = this.children[i].Execute(row1, row2);
        return values;
    }
}

}
