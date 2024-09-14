using System.Text;
using WYF.SqlParser;

namespace WYF.Orm.Query.G.Visitor
{
    public sealed class ConcatCalc : Calc
    {
        private Calc[] children;

        public ConcatCalc(Expr expr, Calc[] children) : base(expr)
        {
            this.children = children;
        }

        public override object ExecuteImpl(IRowFeature row1, IRowFeature row2)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var child in children)
            {
                object v = child.Execute(row1, row2);
                if (v != null)
                {
                    sb.Append(v.ToString());
                }
            }
            return sb.ToString();
        }
    }
}