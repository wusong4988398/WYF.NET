using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.bind
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
