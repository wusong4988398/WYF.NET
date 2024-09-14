using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public abstract class Node
    {
        protected readonly Optional<NodeLocation> location;
        public Optional<NodeLocation> Location { get { return location; } }
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
}
