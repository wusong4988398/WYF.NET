namespace WYF.SqlParser
{
    public interface ICalcCompileable
    {
        Calc Compile(CompileContext context);
    }
}