using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.calc
{
    public class ArrayCalc : Calc
    {
        private Calc[] children;

        public ArrayCalc(Expr expr, Calc[] children) : base(expr)
        {
            this.children = children;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            object[] values = new object[this.children.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = this.children[i].Execute(row1, row2);
            }
            return values;
        }
    }
}
