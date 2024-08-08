using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    public class EntityItemProperty
    {
        public EntityItem EntityItem { get; set; }

        public string PropertyName { get; set; }

        public IDataEntityProperty PropertyType { get; set; }

        public EntityItemProperty(EntityItem entityItem, IDataEntityProperty property, Dictionary<String, IDataEntityType> entityTypeCache)
        {
            this.EntityItem = entityItem;
            this.PropertyName = property.Name;
            this.PropertyType = property;
        }
    }
}
