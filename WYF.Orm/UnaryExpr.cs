namespace WYF.SqlParser
{
    public abstract class UnaryExpr : Expr
    {
        protected UnaryExpr(Optional<NodeLocation> location, Expr child, DataType inputType) : base(location, child, inputType)
        {
        }

        public Expr GetChild()
        {
            return this._children[0];
        }

    }
}