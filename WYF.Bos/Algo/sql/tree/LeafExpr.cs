using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public abstract class LeafExpr : Expr
    {
    public LeafExpr(Optional<NodeLocation> location):base(location, null, null) 
    {
       
    }
}
}
