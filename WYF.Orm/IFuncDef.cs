namespace WYF.SqlParser
{
    public interface IFuncDef
    {
        string Name { get; }
        Expr CreateExpr(Expr[] exprArr);

    }
}