

using WYF.SqlParser;

namespace WYF.Orm.Query.G.Visitor
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
