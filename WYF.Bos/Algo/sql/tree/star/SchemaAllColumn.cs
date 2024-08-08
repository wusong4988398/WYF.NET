using Antlr4.Runtime.Misc;
using WYF.Bos.algo.sql.schema;
using WYF.Bos.algo.sql.tree.bind;
using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;
namespace WYF.Bos.algo.sql.tree.star
{
    public class SchemaAllColumn : Expr, IAllColumn
    {
        public SchemaAllColumn(Optional<NodeLocation> location, ISchema schema) 
            : base(location, ExtractExprs(schema), ExtractTypes(schema))
        {

        }

        private static Expr[] ExtractExprs(ISchema schema)
        {
            ArrayList<Expr> result = new ArrayList<Expr>();
            HashSet<string> duplication = new HashSet<string>();
            foreach (ITable rel in schema.Tables)
            {
                ExtractExprs(rel, result, duplication);
            }
            return result.ToArray();


        }

        private static void ExtractExprs(ITable relation, List<Expr> result, HashSet<string> duplication)
        {
            IColumn[] columns = relation.Columns;
            for (int i = 0; i < columns.Length; i++)
            {
                if (!duplication.Add(columns[i].Name))
                    throw new Exception($"'*' is not allowed, because duplication field name {columns[i].Name} in two dataset.");
                result.Add(new ColumnRef(new Optional<NodeLocation>(), columns[i], null));
            }
        }

        private static void ExtractTypes(ITable relation, List<DataType> result, HashSet<string> duplication)
        {
            IColumn[] columns = relation.Columns;
            for (int i = 0; i < columns.Length; i++)
                result.Add(columns[i].DataType);
        }

        private static DataType[] ExtractTypes(ISchema schema)
        {
            ArrayList<DataType> result =new ArrayList<DataType>();
            HashSet<String> duplication = new HashSet<string>();
            foreach (ITable rel in schema.Tables)
            {
                ExtractTypes(rel, result, duplication);
            }
            return result.ToArray();
        }

        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return default(R);
        }

        public override Calc Compile(CompileContext context)
        {
            return (Calc)new NotSupportCalc(this.GetType().Name);
        }

        public ColumnRef[] GetAll()
        {
            return (ColumnRef[])this._children;
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return "";
        }
    }
}
