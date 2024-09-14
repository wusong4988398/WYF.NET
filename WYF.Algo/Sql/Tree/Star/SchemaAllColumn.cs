using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.bind;
using WYF.Algo.Sql.Tree.calc;
using WYF.Algo.Sql.Schema;

namespace WYF.Algo.Sql.Tree.Star
{
    public class SchemaAllColumn : Expr, IAllColumn
    {
        public SchemaAllColumn(Optional<NodeLocation> location, ISchema schema)
            : base(location, ExtractExprs(schema), ExtractTypes(schema))
        {
        }

        private static Expr[] ExtractExprs(ISchema schema)
        {
            var result = new List<Expr>();
            var duplication = new HashSet<string>();
            foreach (var rel in schema.Tables)
            {
                ExtractExprs(rel, result, duplication);
            }
            return result.ToArray();
        }

        private static DataType[] ExtractTypes(ISchema schema)
        {
            var result = new List<DataType>();
            var duplication = new HashSet<string>();
            foreach (var rel in schema.Tables)
            {
                ExtractTypes(rel, result, duplication);
            }
            return result.ToArray();
        }

        private static void ExtractExprs(ITable relation, List<Expr> result, HashSet<string> duplication)
        {
            var columns = relation.Columns;
            foreach (var column in columns)
            {
                if (!duplication.Add(column.Name))
                {
                    throw new AlgoException($"'*' is not allowed, because duplication field name {column.Name} in two datasets.");
                }
                result.Add(new ColumnRef(Optional<NodeLocation>.Empty(), column, null));
            }
        }

        private static void ExtractTypes(ITable relation, List<DataType> result, HashSet<string> duplication)
        {
            var columns = relation.Columns;
            foreach (var column in columns)
            {
                result.Add(column.DataType);
            }
        }

  

   

     

        public override string Sql()
        {
            return null;
        }

        public ColumnRef[] GetAll()
        {
            return this.GetChildren().Cast<ColumnRef>().ToArray();
        }

        public override Calc Compile(CompileContext context)
        {
            return new NotSupportCalc(this, GetType().Name);
        }

        public override DataType CreateDataType()
        {
            return null;
        }

     

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return default(R);
        }
    }
}
