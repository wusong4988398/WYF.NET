using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Schema;
using WYF.Algo.Sql.Tree.bind;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree
{
    public class UnresolvedAttribute : Attribute, IUnresolved
    {
        private List<string> _nameParts;
        private string _fullName;

        public List<string> NameParts { get { return _nameParts; } }

        public UnresolvedAttribute(Optional<NodeLocation> location, params string[] nameParts) : base(location)
        {
            _nameParts = new List<string>();
            foreach (var name in nameParts)
            {
                _nameParts.Add(name);
            }
            _fullName = string.Join(".", nameParts);
        }

        public UnresolvedAttribute(Optional<NodeLocation> location, List<String> nameParts) : base(location)
        {
            _nameParts = nameParts;
            _fullName = string.Join(".", nameParts);
        }



        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitUnresolvedAttribute(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            throw new AlgoException("Don't call me.");

        }

        public override DataType CreateDataType()
        {
            return DataType.UnknownType;

        }

        public Expr Resolve(ISchema schema)
        {
            return Resolve0(schema, true);
        }
        public Expr Resolve(ISchema schema, IDictionary<string, object> parameters)
        {
            object value;
            if (parameters != null && parameters.TryGetValue(_fullName, out value))
            {
                return new ParamRef(GetLocation(), _fullName, value);
            }
            return Resolve(schema);
        }
        public Expr Resolve(ISchema schema, ISchema right, bool checkAmbiguous)
        {
            Expr expr = Resolve0(schema, false);
            if (expr != null)
            {
                return expr;
            }
            Expr expr2 = Resolve0(right, false);
            if (expr2 == null)
            {
                throw new AlgoException("Illegal field: " + _fullName);
            }
            return expr2;
        }

        private Expr Resolve0(ISchema schema, bool exception)
        {

            object t = schema[_nameParts.ToArray()];
            if (t == null && _nameParts.Count == 1)
            {
                t = Keyword.Of(_nameParts[0]);
                if (t != null)
                {
                    return (Expr)t;
                }
            }
            if (t == null)
            {
                if (exception)
                {
                    string message = "Illegal field: " + Sql();
                    if (GetLocation().IsPresent())
                    {
                        message += ", at " + Location.Value.Text;
                    }
                    throw new AlgoException(message);
                }
                return null;
            }
            if (t is IColumn column)
            {
                return new ColumnRef(Location, column, _fullName);
            }
            if (t is ITable table)
            {
                return new RelationRef(Location, table);
            }
            return null;
        }

        public override string Sql()
        {
            return this._fullName;

        }
    }
}
