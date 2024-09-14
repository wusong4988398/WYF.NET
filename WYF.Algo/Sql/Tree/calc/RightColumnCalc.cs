using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.calc
{
    public sealed class RightColumnCalc : Calc
    {
        private int _index;

        public RightColumnCalc(Expr expr, int index)
            : base(expr)
        {
            _index = index;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            if (row2 == null)
            {
                return null;
            }
            return row2.Get(_index);
        }
    }
}
