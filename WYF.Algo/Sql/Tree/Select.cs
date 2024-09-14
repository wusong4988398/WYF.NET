using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public class Select : Node
    {
        private readonly bool distinct;
        private List<SelectItem> selectItems;

        public Select(bool distinct, List<SelectItem> selectItems) : this(Optional<NodeLocation>.Empty(), distinct, selectItems)
        {
        }

        public Select(NodeLocation location, bool distinct, List<SelectItem> selectItems) : this(Optional<NodeLocation>.Of(location), distinct, selectItems)
        {
        }

        public Select(Optional<NodeLocation> location, bool distinct, List<SelectItem> selectItems) : base(location)
        {
            this.distinct = distinct;
            this.selectItems = selectItems.ToList();
        }

        public bool IsDistinct()
        {
            return distinct;
        }

        public List<SelectItem> GetSelectItems()
        {
            return selectItems;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return visitor.VisitSelect(this, context);
        }

        public override List<Expr> GetChildren()
        {
            return selectItems.Cast<Expr>().ToList();
        }

        public override string ToString()
        {
            //return MoreObjects.ToStringHelper(this).Add("distinct", distinct).Add("selectItems", selectItems).OmitNullValues().ToString();
            return "";
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            Select select = (Select)o;
            return distinct == select.distinct && Equals(selectItems, select.selectItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(distinct, selectItems);
        }

        public override void ReplaceChild(int paramInt, Node paramNode)
        {
            throw new NotImplementedException();
        }
    }
}
