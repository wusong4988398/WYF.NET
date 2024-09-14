namespace WYF.SqlParser
{
    public abstract class BindRef<T> : LeafExpr
    {
        protected T Ref;

        public BindRef(Optional<NodeLocation> location, T @ref) : base(location)
        {
            Ref = @ref;
        }

        public T GetRef()
        {
            return Ref;
        }
    }
}