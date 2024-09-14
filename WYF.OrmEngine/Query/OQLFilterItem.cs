using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query
{
    public class OQLFilterItem
    {
        public virtual string EntityKey { get; set; }

        public string FilterString { get; set; }
    }
}
