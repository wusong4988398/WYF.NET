using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.Orm.Query
{
    public class EntityItemProperty
    {
        private EntityItem _entityItem;
        private string _propertyName;
        private IDataEntityProperty _propertyType;

        public EntityItemProperty(EntityItem entityItem, IDataEntityProperty property, IDictionary<string, IDataEntityType> entityTypeCache)
        {
            _entityItem = entityItem;
            _propertyName = property.Name;
            _propertyType = property;
        }

        public override string ToString()
        {
            return _propertyName;
        }

        public EntityItem EntityItem
        {
            get => _entityItem;
            set => _entityItem = value;
        }

        public string PropertyName
        {
            get => _propertyName;
        }

        public IDataEntityProperty PropertyType
        {
            get => _propertyType;
        }
    }
}
