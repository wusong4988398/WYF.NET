

namespace WYF.SqlParser
{
    public sealed class CastCalc : Calc
    {
        private Calc child;
        private DataType dataType;

        public CastCalc(Expr expr, Calc child, DataType dataType) : base(expr)
        {
            this.child = child;
            this.dataType = dataType;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            return DataType.ConvertValue(this.dataType, this.child.Execute(row1, row2));
        }
    }
}