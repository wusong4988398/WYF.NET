using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public abstract class Node
    {
        protected readonly Optional<NodeLocation> location;

        protected Node(Optional<NodeLocation> location)
        {
            
            this.location = location;
        }

        public abstract  R Accept<R, C>(AstVisitor<R, C> visitor, C context);

        public Optional<NodeLocation> GetLocation()
        {
            return this.location;
        }

        public abstract List<Expr> GetChildren();

        public abstract void ReplaceChild(int paramInt, Node paramNode);
    }
}
