
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.bind;
using WYF.Algo.Sql.Tree.Star;

namespace WYF.Algo.Sql.Tree
{
    public abstract class IAstVisitor<R, C> : IAstExprVisitor<R, C>, IAstStatementVisitor<R, C>
    {
        public R Process(Node node)
        {
            return Process(node, default);
        }

        public R Process(Node node, C c)
        {
            return (R)node.Accept(this, c);
        }

        public virtual R VisitAlias(Alias node, C context)
        {
            return VisitUnaryExpr(node, context);
        }

        public R VisitAttribute(Attribute node, C context)
        {
            return VisitLeafExpr(node, context);

        }

        public R VisitBinaryExpr(BinaryExpr node, C context)
        {
            return VisitExpr(node, context);

        }

        public virtual R VisitBinaryOperator(BinaryOperator node, C context)
        {
            return VisitBinaryExpr(node, context);

        }

        public R VisitBindRef<T>(BindRef<T> node, C context)
        {
            return VisitLeafExpr(node, context);

        }

        public virtual R VisitCast(Cast node, C context)
        {
            return VisitUnaryExpr(node, context);

        }

        public R VisitColumnRef(ColumnRef node, C context)
        {
            return VisitBindRef(node, context);

        }

        public R VisitExpr(Expr node, C context)
        {
            return VisitNode(node, context);
        }

        public virtual R VisitExprList(ExprList node, C context)
        {
            return VisitExpr(node, context);
        }

        public R VisitLeafExpr(LeafExpr node, C context)
        {
            return VisitExpr(node, context);

        }

        public abstract R VisitNode(Node node, C context);

        public R VisitQueryBody(QueryBody queryBody, C context)
        {
            return VisitRelation(queryBody, context);

        }

        public R VisitRelation(Relation relation, C context)
        {
            return VisitNode(relation, context);
        }

        public R VisitRelationAllColumn(RelationAllColumn relationAllColumn, C context)
        {
            return VisitExpr(relationAllColumn, context);
        }

        public R VisitRelationRef(RelationRef node, C context)
        {
            return VisitBindRef(node, context);

        }

        public R VisitSelect(Select select, C context)
        {
            return VisitNode(select, context);
        }

        public R VisitUnaryExpr(UnaryExpr node, C context)
        {
            return VisitExpr(node, context);
        }

        public virtual R VisitUnresolvedAttribute(UnresolvedAttribute node, C context)
        {
            return VisitAttribute(node, context);

        }

        public R VisitUnresolvedStar(UnresolvedStar node, C context)
        {
            return VisitAttribute(node, context);
        }
    }
}
