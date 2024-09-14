using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.dynamicobject;
using WYF.OrmEngine.Impl;

namespace WYF.OrmEngine.Query
{
    public class EntityItemJoinProperty
    {
        public EntityItem ParentEntityItem { get; private set; }

        public IDataEntityProperty ParentOriginProperty { get; private set; }

        public IDataEntityProperty ParentJoinProperty { get; private set; }

        public IDataEntityProperty SubJoinProperty { get; private set; }

        public bool IsIJoinProperty { get; private set; }

        public EntityItemJoinProperty(EntityItem parentEntityItem, IDataEntityProperty property, Dictionary<string, IDataEntityType> entityTypeCache)
        {
            this.ParentEntityItem = parentEntityItem;
            this.ParentOriginProperty = property;
            if (property is IJoinProperty)
            {
                this.ParentJoinProperty = ((IJoinProperty)property).FKProperty;
                this.SubJoinProperty = ((IJoinProperty)property).JoinProperty;
                this.IsIJoinProperty = true;
            }
            else
            {
                this.ParentJoinProperty = property;
                if (property is ICollectionProperty)
                {
                    this.SubJoinProperty = (IDataEntityProperty)parentEntityItem.EntityType.PrimaryKey;
                }
                else if (property is IComplexProperty)
                {
                    IComplexProperty cp = (IComplexProperty)property;
                    IDataEntityType type = cp.ComplexType;
                    if (cp.ComplexType == null)
                        type = ORMConfiguration.InnerGetBaseDataEntityType(cp, entityTypeCache);
                    this.SubJoinProperty = (IDataEntityProperty)type.PrimaryKey;
                }
                else
                {
                    throw new ArgumentException($"不支持该join属性：{property}");
                }
            }

            while (ORMConfiguration.IsEntryEntityType(this.SubJoinProperty.Parent))
                this.SubJoinProperty = (IDataEntityProperty)this.SubJoinProperty.Parent.Parent.PrimaryKey;
        }


        public EntityItemJoinProperty(EntityItem parentEntityItem, IDataEntityProperty parentJoinProperty, IDataEntityProperty subJoinProperty)
        {
            this.ParentEntityItem = parentEntityItem;
            this.ParentOriginProperty = parentJoinProperty;
            this.ParentJoinProperty = parentJoinProperty;
            this.SubJoinProperty = subJoinProperty;
        }

    }
}
