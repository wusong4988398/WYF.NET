using WYF.Bos.algo.sql.g;
using WYF.Bos.algo.sql.schema;
using WYF.Bos.algo.sql.tree.calc;
using WYF.Bos.algo.sql.tree.star;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public class UnresolvedStar : Attribute, IUnresolved
    {
        private string _prefix;

        public string Prefix { get { return _prefix; } }
        public UnresolvedStar(Optional<NodeLocation> location, string prefix) : base(location)
        {
            this._prefix = prefix;
        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitUnresolvedStar(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            throw new Exception("Don't call me.");
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public Expr Resolve(ISchema schema)
        {
            if (this._prefix != null)
            {
                ITable relation = schema.GetTable(this._prefix);
                if (relation == null) throw new Exception("Illegal \"" + this._prefix + ".*\"");
                return (Expr)new RelationAllColumn(GetLocation(), relation);
            }
            return (Expr)new SchemaAllColumn(GetLocation(), schema);
        }

        public override string Sql()
        {
            return string.IsNullOrEmpty(this._prefix) ? "*" : this._prefix + ".*";

        }
    }
}
