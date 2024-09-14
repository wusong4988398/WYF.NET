using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public abstract class LeafExpr : Expr
    {
        public LeafExpr(Optional<NodeLocation> location) : base(location, (Expr[])null, (DataType[])null)
        {

        }
    }
}
