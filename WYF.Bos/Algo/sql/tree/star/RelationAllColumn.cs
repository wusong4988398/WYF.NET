using WYF.Bos.algo.sql.g;
using WYF.Bos.algo.sql.schema;
using WYF.Bos.algo.sql.tree.bind;
using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;


namespace WYF.Bos.algo.sql.tree.star
{
    public class RelationAllColumn : Expr, IAllColumn
    {
        public RelationAllColumn(Optional<NodeLocation> location, ITable relation) 
            : base(location, ExtractExprs(relation), ExtractTypes(relation))
        {

        }
        private static Expr[] ExtractExprs(ITable relation)
        {
            IColumn[] columns = relation.Columns;
            Expr[] result = new Expr[columns.Length];
            for (int i = 0; i < columns.Length; i++)
                result[i] = (Expr)new ColumnRef(new Optional<NodeLocation>(), columns[i], null);
            return result;
        }

        private static DataType[] ExtractTypes(ITable relation)
        {
            IColumn[] columns = relation.Columns;
            DataType[] result = new DataType[columns.Length];
            for (int i = 0; i < columns.Length; i++)
                result[i] = columns[i].DataType;
            return result;
        }


        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return (R)visitor.VisitRelationAllColumn(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return (Calc)new NotSupportCalc(this.GetType().Name);
        }

        ColumnRef[] IAllColumn.GetAll()
        {
            return (ColumnRef[])this._children;
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return null;
        }

   
    }
}
