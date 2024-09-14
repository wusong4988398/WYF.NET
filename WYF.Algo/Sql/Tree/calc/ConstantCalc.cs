using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.calc
{
    public sealed class ConstantCalc : Calc
    {
        private readonly object value;

        public ConstantCalc(Expr expr, object value) : base(expr)
        {
            this.value = value;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            return this.value;
        }

        public object GetValue()
        {
            return this.value;
        }
    }
}
