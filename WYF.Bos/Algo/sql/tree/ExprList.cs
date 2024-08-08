using WYF.Bos.algo.datatype;
using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public class ExprList : Expr
    {
        public ExprList(Optional<NodeLocation> location, Expr[] children) : base(location, children, RepeatDataTypes((DataType)AnyType.Instance, (children == null) ? 0 : children.Length))
        {

        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitExprList(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return (Calc)new ArrayCalc(CompileChildren(context));
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return JoinChildrenSql(this._children);
        }
    }
}
