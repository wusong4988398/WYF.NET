using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.SqlParser
{
    public abstract class Attribute : LeafExpr
    {
        public Attribute(Optional<NodeLocation> location) : base(location)
        {

        }
    }
}
