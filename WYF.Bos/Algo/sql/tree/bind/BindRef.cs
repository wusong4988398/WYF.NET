using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.bind
{
    public abstract class BindRef<T> : LeafExpr
    {
        protected T _refer;
  
        public BindRef(Optional<NodeLocation> location, T refer):base(location) 
        {
            
            this._refer = refer;
        }

        public T GetRef()
        {
            return this._refer;
        }
    }
}
