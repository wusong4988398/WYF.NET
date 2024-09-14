using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public abstract class SelectItem : Node
    {
        public SelectItem(Optional<NodeLocation> location) : base(location)
        {
        }
    }
}
