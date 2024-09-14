using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree
{
    public class Concat : Expr
    {
        public Concat(Optional<NodeLocation> location, Expr[] children)
            : base(location, children, RepeatDataTypes(DataType.AnyType, children == null ? 0 : children.Length))
        {
        }

        public override string Sql()
        {
            return Location.Get().Text;
        }

        public override DataType CreateDataType()
        {
            return DataType.StringType;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return default;
        }

        public override Calc Compile(CompileContext context)
        {
            return new ConcatCalc(this, CompileChildren(context));
        }
    }
}
