using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public abstract class QueryBody : Relation
    {
        public QueryBody(Optional<NodeLocation> location) : base(location)
        {
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitQueryBody(this, context);
        }
    }
}
