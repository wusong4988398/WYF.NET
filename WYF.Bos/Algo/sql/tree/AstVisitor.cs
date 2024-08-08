using WYF.Bos.algo.sql.schema;
using WYF.Bos.algo.sql.tree.bind;
using WYF.Bos.algo.sql.tree.star;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public interface AstVisitor<R, C> : AstExprVisitor<R, C>, AstStatementVisitor<R, C>
    {
        R Process(Node node)
        {
           
            return Process(node, default(C));
        }

        R Process(Node node, C context)
        {
            return node.Accept(this, context);
        }

        R VisitExpr(Expr node, C context)
        {
            return VisitNode(node, context);
        }

        R? VisitNode(Node node, C context)
        {
            return default(R);
        }

        R VisitAlias(Alias node, C context)
        {
            return VisitUnaryExpr(node, context);
        }
         R VisitUnaryExpr(UnaryExpr node, C context)
        {
            return VisitExpr(node, context);
        }

         R VisitUnresolvedStar(UnresolvedStar node, C context)
        {
            return VisitAttribute(node, context);
        }
           R VisitAttribute(Attribute node, C context)
        {
            return VisitLeafExpr(node, context);
        }
          R VisitLeafExpr(LeafExpr node, C context)
        {
            return VisitExpr(node, context);
        }
          R VisitColumnRef<T>(ColumnRef node, C context)
        {
            return VisitBindRef((BindRef<IColumn>)node, context);
        }
           R VisitBindRef<T>(BindRef<T> node, C context)
        {
            return VisitLeafExpr((LeafExpr)node, context);
        }

           R VisitRelationAllColumn(RelationAllColumn relationAllColumn, C context)
        {
            return VisitExpr((Expr)relationAllColumn, context);
        }

           R VisitUnresolvedAttribute(UnresolvedAttribute node, C context)
        {
            return VisitAttribute(node, context);
        }
           R VisitRelationRef(RelationRef node, C context)
        {
            return VisitBindRef(node, context);
        }

           R VisitExprList(ExprList node, C context)
        {
            return VisitExpr(node, context);
        }

    }
}
