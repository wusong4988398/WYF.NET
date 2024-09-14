using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree;

namespace WYF.OrmEngine.Query.OQL.G.Visitor
{
    public abstract class ExprVisitor<CONTEXT> : IAstVisitor<object, CONTEXT>
    {
        public abstract object DefaultVisit(Expr expr, CONTEXT context);

        public override object VisitNode(Node node, CONTEXT context)
        {
            if (node is Expr)
            {
                return DefaultVisit((Expr)node, context);
            }
            return null;
        }
    }
}
