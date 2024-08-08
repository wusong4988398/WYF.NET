using WYF.Bos.algo.sql.g;
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
    public class ColumnRef : BindRef<IColumn>
    {
        private string _alias;

        private bool _right;

        private int _index;

        public ColumnRef(Optional<NodeLocation> location, IColumn refer, string alias):base(location, refer)
        {
           
            this._alias = (string.IsNullOrEmpty(alias)) ? refer.FullName : alias;
            this._right = "right" == refer.Table.Name;
            this._index = refer.Index;
        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return (R)visitor.VisitColumnRef<IColumn>(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            if (this._right)
                return (Calc)new RightColumnCalc(this._index);
            return (Calc)new LeftColumnCalc(this._index);
        }

        public override DataType GetDataType()
        {
            return this._refer.DataType;
        }

        public override string Sql()
        {
            return this._alias;
        }
    }
}
