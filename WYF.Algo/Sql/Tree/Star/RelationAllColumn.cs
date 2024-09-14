using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Schema;
using WYF.Algo.Sql.Tree.bind;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree.Star
{
    public class RelationAllColumn : Expr, IAllColumn
    {
        public RelationAllColumn(Optional<NodeLocation> location, ITable relation)
            : base(location, ExtractExprs(relation), ExtractTypes(relation))
        {
        }

        private static Expr[] ExtractExprs(ITable relation)
        {
            var columns = relation.Columns;
            var result = new Expr[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                result[i] = new ColumnRef(Optional<NodeLocation>.Empty(), columns[i], null);
            }
            return result;
        }

        private static DataType[] ExtractTypes(ITable relation)
        {
            var columns = relation.Columns;
            var result = new DataType[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                result[i] = columns[i].DataType;
            }
            return result;
        }

        public override DataType CreateDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return null;
        }

        public ColumnRef[] GetAll()
        {
            return (ColumnRef[])this.GetChildren().ToArray();
        }

        public override Calc Compile(CompileContext context)
        {
            return new NotSupportCalc(this, GetType().Name);
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitRelationAllColumn(this, context);
        }
    }
}
