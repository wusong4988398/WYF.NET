namespace WYF.SqlParser
{
    public class RelationRef : BindRef<ITable>
    {
        public RelationRef(Optional<NodeLocation> location, ITable @ref)
            : base(location, @ref)
        {
        }

        public override DataType GetDataType()
        {
            return null;
        }

        public override DataType CreateDataType()
        {
            return null;
        }

        public override string Sql()
        {
            return ((ITable)this.Ref).Name;
        }

        public override TR Accept<TR, TC>(IAstVisitor<TR, TC> visitor, TC context)
        {
            return visitor.VisitRelationRef(this, context);
        }

        public override Calc Compile(CompileContext context)
        {
            return new NotSupportCalc(this, typeof(RelationRef).Name);
        }

     
    }
}