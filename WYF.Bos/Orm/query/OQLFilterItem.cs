using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    public class OQLFilterItem
    {
        public virtual string EntityKey { get; set; }

        public string FilterString { get; set; }
    }
}
