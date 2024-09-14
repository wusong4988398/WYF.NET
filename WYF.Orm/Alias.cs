namespace WYF.SqlParser
{
    public class Alias : UnaryExpr
    {
        private string alias;

        public Alias(Optional<NodeLocation> location, Expr child, string alias) : base(location, child, AnyType.Instance)
        {
            this.alias = alias;
        }

        public string GetAlias()
        {
            return this.alias;
        }

        public override string Sql()
        {
            return GetChild().Sql() + " AS " + this.alias;
        }

        public override DataType CreateDataType()
        {
            return GetChild().GetDataType();
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitAlias(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return GetChild().Compile(context);
        }

     
    }
}