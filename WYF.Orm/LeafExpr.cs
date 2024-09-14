namespace WYF.SqlParser
{
    public abstract class LeafExpr : Expr
    {
        public LeafExpr(Optional<NodeLocation> location) : base(location, (Expr[])null, (DataType[])null)
        {

        }
    }
}