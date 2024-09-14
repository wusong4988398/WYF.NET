using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree
{
    public abstract class Expr : Node, IInterpret, ICalcCompileable
    {

        protected Expr[] _children;
        protected DataType[] inputTypes;
        private bool hasCheckAcceptsTypes;
        private DataType dataType;

        public abstract DataType CreateDataType();

        protected Expr(Optional<NodeLocation> location) : base(location)
        {

        }

        public Expr(Optional<NodeLocation> location, Expr child, DataType inputType) : this(location, new Expr[] { child }, new DataType[] { inputType })
        {

        }

        public Expr(Optional<NodeLocation> location, Expr[] children, DataType[] inputTypes) : base(location)
        {

            this.hasCheckAcceptsTypes = false;
            this._children = children;
            this.inputTypes = inputTypes;
        }





        public override List<Expr> GetChildren()
        {
            if (this._children != null)
            {
                return this._children.ToList();
            }
            return new List<Expr>();
        }

        public Expr GetChildren(int index)
        {
            if (this._children != null)
            {
                return this._children[index];
            }
            return GetChildren()[index];
        }


        public int ChildrenCount
        {
            get
            {
                if (this._children == null)
                {
                    return this.GetChildren().Count;
                }
                return _children.Length;
            }
        }
        public string JoinChildrenSql(Expr[] children)
        {
            return JoinChildrenSql(children, ",");
        }
        public String JoinChildrenSql(Expr[] children, String delimiter)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < children.Length; i++)
            {
                if (i > 0)
                    sb.Append(delimiter);
                sb.Append(children[i].Sql());
            }
            return sb.ToString();
        }
        public override void ReplaceChild(int index, Node expr)
        {
            ReplaceChild(index, (Expr)expr);
        }
        public static DataType[] RepeatDataTypes(DataType dataType, int n)
        {
            if (n == 0)
                return null;
            DataType[] inputTypes = new DataType[n];
            for (int i = 0; i < inputTypes.Length; i++)
                inputTypes[i] = dataType;
            return inputTypes;
        }

        public void ReplaceChild(int index, Expr expr)
        {
            if (this._children == null)
                throw new Exception("children is null.");
            if (index > this._children.Count() || index < 0)
                throw new Exception($"Index {index} out of children bound [{0}:{this._children.Count()}]");
            this._children[index] = expr;
        }

        public override String ToString()
        {
            if (this.location.IsPresent())
            {
                return this.location.Get().Text;
            }
            return Sql();
        }


        protected Calc[] CompileChildren(CompileContext context)
        {
            if (this._children != null)
            {
                Calc[] result = new Calc[this._children.Length];
                for (int i = 0; i < this._children.Length; i++)
                {
                    if (this._children[i] != null)
                        result[i] = this._children[i].Compile(context);
                }
                return result;
            }
            return null;
        }

        public Calc CompileChildren(CompileContext ctx, int index)
        {
            return CompileChildren(ctx, this._children[index], this.inputTypes[index]);
        }

        private Calc CompileChildren(CompileContext ctx, Expr child, DataType inputType)
        {
            CheckAcceptsTypes();
            return child.Compile(ctx);
        }
        private void CheckAcceptsTypes()
        {
            if (this.hasCheckAcceptsTypes)
            {
                return;
            }
            for (int i = 0; i < this._children.Length; i++)
            {
                CheckAcceptsTypes(this._children[i], this.inputTypes[i]);
            }
            this.hasCheckAcceptsTypes = true;
        }
        private void CheckAcceptsTypes(Expr child, DataType inputType)
        {
            if (!inputType.AcceptsType(child.GetDataType()))
            {
                throw new AlgoException($"expr {child.ToString()} should be {inputType.GetName()}, but {child.GetDataType().GetName()} found.");
            }
        }





        public abstract string Sql();

        public virtual DataType GetDataType()
        {
            if (this.dataType == null)
            {
                this.dataType = CreateDataType();
            }
            return this.dataType;

        }
        public abstract Calc Compile(CompileContext context);
    }
}
