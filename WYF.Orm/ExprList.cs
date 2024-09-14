namespace WYF.SqlParser
{
    public class ExprList : Expr
    {
        public ExprList(Optional<NodeLocation> location, Expr[] children) : base(location, children, RepeatDataTypes(AnyType.Instance, children == null ? 0 : children.Length))
        {
        }

        public override string Sql()
        {
            return JoinChildrenSql(this._children);
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override DataType CreateDataType()
        {
            return null;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitExprList(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return new ArrayCalc(this, CompileChildren(context));
        }
    }
}