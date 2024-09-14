using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Schema;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree.bind
{
    public class RelationRef : BindRef<ITable>
    {
        public RelationRef(Optional<NodeLocation> location, ITable @ref)
            : base(location, @ref)
        {
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override DataType CreateDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return ((ITable)this.Ref).Name;
        }

        public override TR Accept<TR, TC>(IAstVisitor<TR, TC> visitor, TC context)
        {
            return visitor.VisitRelationRef(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return new NotSupportCalc(this, typeof(RelationRef).Name);
        }


    }
}
