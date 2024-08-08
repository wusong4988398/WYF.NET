using WYF.Bos.algo.sql.schema;
using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.bind
{
    public class RelationRef : BindRef<ITable>
    {
        public RelationRef(Optional<NodeLocation> location, ITable refer) : base(location, refer)
        {
        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return (R)visitor.VisitRelationRef(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return (Calc)new NotSupportCalc(typeof(RelationRef).Name);
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return this._refer.Name;
        }
    }
}
