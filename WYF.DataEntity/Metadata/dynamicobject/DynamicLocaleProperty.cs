using WYF.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.DataEntity.Metadata.dynamicobject
{
    public class DynamicLocaleProperty: DynamicCollectionProperty
    {
        public DynamicLocaleProperty() { }
        public override Type PropertyType => typeof(LocaleDynamicObjectCollection);

        
        public override object GetDTValueFast(DynamicObject obj)
        {
            IDataStorage dataStorage = obj.DataStorage;
            object localValue = dataStorage.getLocalValue(this);
            if (localValue == null)
            {
                lock (obj)
                {
                    localValue = dataStorage.getLocalValue(this);
                    if (localValue == null)
                    {
                        localValue = new LocaleDynamicObjectCollection(this._collectionItemPropertyType, obj);
                        dataStorage.setLocalValue(this, localValue);
                    }
                }
            }
            else
            {
                var coll = (LocaleDynamicObjectCollection)localValue;
                if (coll.Count == 0 && coll.DynamicObjectType == null)
                {
                    localValue = new LocaleDynamicObjectCollection(this._collectionItemPropertyType, obj);
                    dataStorage.setLocalValue(this, localValue);
                }
            }

            return localValue;
        }
    }
}
