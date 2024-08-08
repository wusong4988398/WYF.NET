
using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.Dynamicobject
{
    /// <summary>
    /// 动态实体的集合属性 集合属性指向另外一个实体类型的引用，所不同的是集合属性表达主表与明细表的关系时候使用。
    ///在数据库中，集合属性会保存在明细表中，为一对多的关系。主实体与集合属性的关联主键使用主实体的主键，即要求明细表的外键名字和主表主键名字相同。
    ////如系统中的订单主表及明细表分录即使用集合属性来表示
    /// </summary>
    [Serializable]
    public class DynamicCollectionProperty : DynamicProperty, ICollectionProperty
    {
        public DynamicObjectType _collectionItemPropertyType;
        public IDataEntityType ItemType
        {
            get { return _collectionItemPropertyType; } 
            set { this._collectionItemPropertyType = (DynamicObjectType)value; }
        }


        public DynamicCollectionProperty() { }

        public DynamicCollectionProperty(String name, DynamicObjectType dynamicItemPropertyType) : base(name, typeof(DynamicObjectCollection), null, true)
        {
            
            if (dynamicItemPropertyType == null) throw new Exception("构造动态集合属性DynamicCollectionProperty失败，构造参数：动态实体属性类型dynamicItemPropertyType不能为空！");
            this._collectionItemPropertyType = dynamicItemPropertyType;
        }

        public DynamicCollectionProperty(String name, DynamicObjectType dynamicItemPropertyType, Type collectionType) : base(name, collectionType, null, true)
        {

            if (dynamicItemPropertyType == null) throw new Exception("构造动态集合属性DynamicCollectionProperty失败，构造参数：动态实体属性类型dynamicItemPropertyType不能为空！");

            this._collectionItemPropertyType = dynamicItemPropertyType;
        }

        public DynamicObjectType DynamicCollectionItemPropertyType { get { return _collectionItemPropertyType; } }
        

        public new object GetDTValueFast(DynamicObject obj)
        {
            IDataStorage dataStorage = obj.DataStorage;
            object localValue = dataStorage.getLocalValue(this);
            if (localValue == null)
                lock (obj)
                {
                    localValue = dataStorage.getLocalValue(this);
                    if (localValue == null)
                    {
                        localValue = new DynamicObjectCollection(this._collectionItemPropertyType, obj);
                        dataStorage.setLocalValue(this, localValue);
                    }
                }
            return localValue;
        }

        public DynamicObjectType GetDynamicCollectionItemPropertyType()
        {
            return this._collectionItemPropertyType;
        }

        protected new bool isEquals(Object obj)
        {
            return (base.isEquals(obj) && this._collectionItemPropertyType
              .Equals(((DynamicCollectionProperty)obj)._collectionItemPropertyType));
        }

        public new int createHashCode()
        {
            return base.createHashCode() ^ this._collectionItemPropertyType.GetHashCode();
        }
        public new bool IsEmpty(object dataEntity)
        {
            var items = GetValue(dataEntity);
            if (items == null)
                return true;
            if (items is DynamicObjectCollection objs)
            {
                if (objs.Count == 0)
                    return true;
                foreach (var item in objs)
                {
                    if (!_collectionItemPropertyType.IsEmpty(item))
                        return false;
                }
            }
            return true;
        }

        public EntryInfo GetEntryInfo(Object dataEntity)
        {
            return ((DynamicObject)dataEntity).DataEntityState.EntryInfos[this.Name];
        }

    }
}
