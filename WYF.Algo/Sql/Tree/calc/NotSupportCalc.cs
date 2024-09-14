using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.calc
{
    public class NotSupportCalc : Calc
    {
        private string _name;

        public NotSupportCalc(Expr expr, string name)
            : base(expr)
        {
            _name = name;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            throw new AlgoException($"Not support calc for {_name}");
        }
    }
}
