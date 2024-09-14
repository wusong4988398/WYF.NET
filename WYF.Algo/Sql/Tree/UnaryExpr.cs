using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public abstract class UnaryExpr : Expr
    {
        protected UnaryExpr(Optional<NodeLocation> location, Expr child, DataType inputType) : base(location, child, inputType)
        {
        }

        public Expr GetChild()
        {
            return this._children[0];
        }

    }
}
