
using System.Collections.Generic;



namespace WYF.SqlParser
{
    public abstract class Node
    {
        protected readonly Optional<NodeLocation> location;
        public Optional<NodeLocation> Location {  get { return location; } }
        protected Node(Optional<NodeLocation> location)
        {

            this.location = location;
        }

        public abstract R Accept<R, C>(IAstVisitor<R, C> visitor, C context);



        public Optional<NodeLocation> GetLocation()
        {
            return this.location;
        }

        public abstract List<Expr> GetChildren();

        public abstract void ReplaceChild(int paramInt, Node paramNode);
    }



    public class Optional : Optional<object>
    {
        #region Private Fields

        private static readonly Optional EMPTY = null;

        #endregion

        new public static Optional Empty()
        {
            return Optional.EMPTY;
        }
    }
    // Define the Optional<T> class if it's not already defined.
    public class Optional<T>
    {
        #region Private Fields

        private static readonly Optional<T> EMPTY = OfNullable(default(T));

        #endregion

        #region Public Properties

        public T Value { get; }

        #endregion

        #region Constructors

        public Optional()
        {
            this.Value = default(T);
        }

        public Optional(T value)
        {
            this.Value = value;
        }

        #endregion

        #region Static Public Methods

        public static Optional<T> Empty()
        {
            return EMPTY;
        }

        public static Optional<T> Of(T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<T> OfNullable(T value)
        {
            return ((value == null) ? Empty() : Of(value));
        }

        #endregion

        #region Public Methods

        public bool IsPresent()
        {
            return this.Value != null;
        }

        public T Get()
        {
            return this.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Optional<T>)
            {
                return this.Equals((Optional<T>)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Optional<T> other)
        {
            if (this.IsPresent() && other.IsPresent())
            {
                return this.Get().Equals(other.Get());
            }
            else
            {
                return this.IsPresent() == other.IsPresent();
            }
        }

        public override int GetHashCode()
        {
            if (this.Value != null)
            {
                return Hashing.Hash(this.Value);
            }
            else
            {
                return base.GetHashCode();
            }
        }

        #endregion
    }
}
