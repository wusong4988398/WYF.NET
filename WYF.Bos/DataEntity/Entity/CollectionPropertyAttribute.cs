using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Entity
{
    public class CollectionPropertyAttribute: Attribute
    {
        public string Name { get; set; }

        public Type CollectionItemPropertyType { get; set; }
    }
}
