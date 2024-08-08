using WYF.Bos.algo.sql.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.oql.g.visitor
{
    public abstract class ExprVisitor<CONTEXT> : AstVisitor<Object, CONTEXT>
    {
        public abstract Object DefaultVisit(Expr paramExpr, CONTEXT paramCONTEXT);

        public Object VisitNode(Node node, CONTEXT context)
        {
            if (node is Expr)
      return DefaultVisit((Expr)node, context);
            return null;
        }
    }
}
