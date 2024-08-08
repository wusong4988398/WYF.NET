using Antlr4.Runtime.Misc;
using WYF.Bos.algo.sql.schema;
using WYF.Bos.algo.sql.tree.bind;
using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;
namespace WYF.Bos.algo.sql.tree
{
    public class UnresolvedAttribute : Attribute, IUnresolved
    {
        private List<string> _nameParts = new List<string>();
        private string _fullName;

        public List<string> NameParts { get { return _nameParts; } }

        public UnresolvedAttribute(Optional<NodeLocation> location,  params string[] nameParts) : base(location)
        {
            foreach (var name in nameParts)
            {
                this._nameParts.Add(name);
            }
            this._fullName = string.Join(".", _nameParts);
        }
        public UnresolvedAttribute(Optional<NodeLocation> location, List<string> nameParts) : base(location)
        {
            
            this._nameParts = nameParts;
            this._fullName = string.Join(".", _nameParts);
        }
        public Expr Resolve(ISchema schema)
        {
            return Resolve0(schema, true);
        }
        private Expr Resolve0(ISchema schema, bool exception)
        {
            
            object t = schema.GetColumn(this._nameParts.ToArray());
            if (t == null && this._nameParts.Count==1)
            {
                t = Keyword.Of(this._nameParts[0]);
                if (t != null) return (Expr)t;
            }
            if (t == null)
            {
                if (exception)
                {
                    string message = "Illegal field:" + this.Sql();
                    if (GetLocation().HasValue)
                    {
                        message = message + ", at " + GetLocation().Value.ToString();
                    }
                    throw new Exception(message);
                }
                return null;
            }

            if (t is IColumn) return (Expr)new ColumnRef(GetLocation(), (IColumn)t, this._fullName);


            if (t is ITable) return (Expr)new RelationRef(GetLocation(), (ITable)t);

            return null;
        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitUnresolvedAttribute(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            throw new Exception("Don't call me.");
        }

        public override DataType GetDataType()
        {
            return (DataType)DataType.UnknownType;
        }



        public override string Sql()
        {
            return this._fullName;
        }
    }
}
