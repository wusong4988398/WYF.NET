using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Schema;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree.bind
{
    public class ColumnRef : BindRef<IColumn>
    {
        private string alias;
        private bool right;
        private int index;

        public ColumnRef(Optional<NodeLocation> location, IColumn @ref, string alias) : base(location, @ref)
        {
            this.alias = alias == null ? @ref.FullName : alias;
            this.right = "right".Equals(@ref.Table.Name);
            this.index = @ref.Index;
        }

        public override DataType CreateDataType()
        {
            return Ref.DataType;
        }

        public override string Sql()
        {
            return alias;
        }

        public string GetAlias()
        {
            return alias;
        }

        public string GetName()
        {
            return Ref.Name;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitColumnRef(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            if (right)
            {
                return new RightColumnCalc(this, index);
            }
            return new LeftColumnCalc(this, index);
        }
    }
}
