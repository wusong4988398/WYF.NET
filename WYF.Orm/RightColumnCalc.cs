namespace WYF.SqlParser
{
    public sealed class RightColumnCalc : Calc
    {
        private int _index;

        public RightColumnCalc(Expr expr, int index)
            : base(expr)
        {
            _index = index;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            if (row2 == null)
            {
                return null;
            }
            return row2.Get(_index);
        }
    }
}