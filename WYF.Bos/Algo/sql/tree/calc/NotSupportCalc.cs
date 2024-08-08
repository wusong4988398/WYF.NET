using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public class NotSupportCalc : Calc
    {
    private string name;

    public NotSupportCalc(string name)
    {
        this.name = name;
    }

    public Object Execute(IRowFeature row1, IRowFeature row2)
    {
        throw new Exception("Not support calc for " + this.name);
    }
}
}
