
using WYF.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.Clr
{
    public sealed class CollectionProperty : DataEntityProperty, ICollectionProperty
    {
        private IDataEntityType _itemType;
        public CollectionProperty(PropertyInfo propertyInfo, int ordinal) : base(propertyInfo, ordinal)
        {
            CollectionPropertyAttribute cpAtt = propertyInfo.GetCustomAttribute<CollectionPropertyAttribute>();
            this.name = !string.IsNullOrEmpty(cpAtt.Name) ? cpAtt.Name : propertyInfo.Name;
            this.Init(cpAtt.CollectionItemPropertyType);
        }

        private void Init(Type itemType)
        {
            this._itemType = DataEntityType.GetDataEntityType(itemType);
        }

        public IDataEntityType ItemType => this._itemType;

        
    }
}
