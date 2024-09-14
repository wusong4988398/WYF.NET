

using WYF.SqlParser;

namespace WYF.Orm.Query.G.Visitor
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