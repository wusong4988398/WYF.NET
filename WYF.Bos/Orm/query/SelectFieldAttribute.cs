using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    [Serializable]
    public class SelectFieldAttribute
    {

        public string FieldName { get; set; }

        public string Key { get; set; }
    }
}
