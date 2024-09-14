using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.SqlParser
{

    public class Cast : UnaryExpr
    {
        private DataType dataType;

        public Cast(Optional<NodeLocation> location, Expr child, DataType dataType)
            : base(location, child, AnyType.Instance)
        {
            this.dataType = dataType.Equals(DataType.DoubleType) ? DataType.BigDecimalType : dataType;
        }

        public override string Sql()
        {
            if (Location.IsPresent() && !string.IsNullOrEmpty(Location.Get().Text))
            {
                return Location.Get().Text;
            }
            return "CAST(" + GetChild().Sql() + " as " + this.dataType.GetName() + ")";
        }

        public override DataType CreateDataType()
        {
            return this.dataType;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitCast(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            
            Calc child = CompileChildren(context, 0);
            //if (child is CaseWhenSearchCalc || child is CaseWhenSearchWithElseCalc || child is CaseWhenSimpleCalc || child is CaseWhenSimpleWithElseCalc)
            //{
            //    return new CastCalc(this, child, this.dataType);
            //}
            
            if (_children[0].GetDataType() == this.dataType)
            {
                return child;
            }
            return new CastCalc(this, child, this.dataType);
        }
    }
}
