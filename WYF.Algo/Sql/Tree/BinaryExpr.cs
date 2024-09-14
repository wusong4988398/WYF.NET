using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public abstract class BinaryExpr : Expr
    {
        public BinaryExpr(Optional<NodeLocation> location, Expr left, Expr right, DataType leftInputType, DataType rightInputType)
            : base(location, new Expr[] { left, right }, new DataType[] { leftInputType, rightInputType })
        {
        }

        public Expr GetLeft()
        {
            return _children[0];
        }

        public Expr GetRight()
        {
            return _children[1];
        }
    }
}
