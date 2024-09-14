

namespace WYF.SqlParser
{
    public interface IFuncFactory
    {
        IFuncDef LookupFunc(string str, Expr[] exprArr);

    }
}