using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.Property;

namespace WYF.Entity
{
    /// <summary>
    /// ORM实体描述
    /// </summary>
    public class MainEntityType : EntityType
    {
        public static readonly MainEntityType Empty = new MainEntityType();

        protected List<RefPropType> _refPropTypes = new List<RefPropType>();

        public long CheckVersionTimeMillis { get; set; }

        [SimpleProperty]
        public string Version { get; set; }

        [SimpleProperty]
        public string BOSVer {  get; set; } 


        /// <summary>
        /// 引用属性类型
        /// </summary>
        [CollectionProperty(CollectionItemPropertyType = typeof(RefPropType))]
        public List<RefPropType> RefPropTypes
        {
            get { return _refPropTypes; }
            set
            {

                _refPropTypes = value;
            }
        }


        protected void FillRefType(DynamicObjectType type, Dictionary<string, DynamicObjectType> types)
        {
            DoFillRefType(type, types);
            FillRefParentProperty(type);
        }

        private void FillRefParentProperty(DynamicObjectType type)
        {
            foreach (IDataEntityProperty prop in type.Properties)
            {
                if (prop is ItemClassProp)
                {

                }
                else if (prop is EntryProp)
                {

                }
            }
        }

        private void DoFillRefType(DynamicObjectType type, Dictionary<string, DynamicObjectType> types)
        {
            List<DynamicProperty> refIds = new List<DynamicProperty>();
            foreach (IDataEntityProperty prop in type.Properties)
            {
                if (prop is EntryProp)
                {
                    DoFillRefType((EntityType)((EntryProp)prop).ItemType, types);

                }
                else if (prop is BasedataProp)
                {
                    FillRefToBaseDataProp(types, refIds, (BasedataProp)prop);

                }
            }
            foreach (DynamicProperty prop in refIds)
            {
                type.AddProperty(prop);
            }
        }

        private void FillRefToBaseDataProp(Dictionary<string, DynamicObjectType> types, List<DynamicProperty> refIds, BasedataProp prop)
        {
            DynamicSimpleProperty pRefId;
            DynamicObjectType dt = types.GetValueOrDefault(prop.BaseEntityId);
            if (dt != null)
            {
                prop.ComplexType = dt;
                if (dt.PrimaryKey.PropertyType == typeof(string))
                {
                    pRefId = new VarcharProp(true);
                }
                else
                {
                    pRefId = new LongProp(true);
                }
                pRefId.IsPrimaryKey = false;
                if (string.IsNullOrEmpty(prop.Alias))
                {
                    pRefId.IsDbIgnore = true;
                }
                else
                {
                    pRefId.Alias = prop.Alias;
                    pRefId.TableGroup = prop.TableGroup;

                }
                pRefId.Name = prop.Name + "_id";
                refIds.Add(pRefId);
                prop.RefIdPropName = prop.Name + "_id";
                prop.RefIdProp = pRefId;
            }

        }

        public override void EndInit()
        {
            base.EndInit();
            if (this._refPropTypes.Count > 0)
            {
                Dictionary<string, DynamicObjectType> types = RefEntityTypeCache.GetRefTypes(this._refPropTypes);
                FillRefType(this, types);
                this._refPropTypes = null;
            }

        }



    }
}
