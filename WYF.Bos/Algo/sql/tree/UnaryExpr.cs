using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public abstract class UnaryExpr : Expr
    {
        protected UnaryExpr(Optional<NodeLocation> location, Expr child, DataType inputType) : base(location)
        {

        }

        public Expr Child
        {
            get { return this.GetChildren()[0]; }
        }
    }
}
