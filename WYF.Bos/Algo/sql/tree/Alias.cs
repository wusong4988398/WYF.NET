using WYF.Bos.algo.datatype;
using WYF.Bos.algo.sql.g;
using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public class Alias : UnaryExpr
    {
        private string _alias;

        public Alias(Optional<NodeLocation> location, Expr child, String alias)
            :base(location, child, (DataType)AnyType.Instance)
        {
            
            this._alias = alias;
        }
        public Alias(Optional<NodeLocation> location, Expr child, DataType inputType) : base(location, child, inputType)
        {
        }

        public string GetAlias()
        {
            return this._alias;
        }

        

        public override DataType GetDataType()
        {
            return this.Child.GetDataType();
        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitAlias(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return this.Child.Compile(context);
        }

      

  

        public override string Sql()
        {
            
            return this.Child.Sql() + " AS " + this._alias;
        }
    }
}
