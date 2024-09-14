

namespace WYF.SqlParser
{
    public abstract class BinaryOperator : BinaryExpr
    {
        protected string Operator;

        public BinaryOperator(Optional<NodeLocation> location, Expr left, Expr right, DataType leftInputType, DataType rightInputType)
            : base(location, left, right, leftInputType, rightInputType)
        {
        }

        public string GetOperator()
        {
            return this.Operator;
        }

        public override string Sql()
        {
            return _children[0].Sql() + Operator + _children[1].Sql();
        }
    }
}