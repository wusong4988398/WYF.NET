using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Schema;
using WYF.Algo.Sql.Tree.calc;
using WYF.Algo.Sql.Tree.Star;

namespace WYF.Algo.Sql.Tree
{
    public class UnresolvedStar : Attribute, IUnresolved
    {
        private string _prefix;

        public string Prefix { get { return _prefix; } }
        public UnresolvedStar(Optional<NodeLocation> location, string prefix) : base(location)
        {
            this._prefix = prefix;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitUnresolvedStar(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            throw new Exception("Don't call me.");
        }

 
        public Expr Resolve(ISchema schema)
        {
            if (this._prefix != null)
            {

                ITable relation = schema[this._prefix];
                if (relation == null) throw new Exception("Illegal \"" + this._prefix + ".*\"");
                return (Expr)new RelationAllColumn(GetLocation(), relation);
            }
            return (Expr)new SchemaAllColumn(GetLocation(), schema);
        }

        public override string Sql()
        {
            return string.IsNullOrEmpty(this._prefix) ? "*" : this._prefix + ".*";

        }

        public override DataType CreateDataType()
        {
            return null;
        }
    }
}
