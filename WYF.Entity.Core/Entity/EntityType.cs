using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.dynamicobject;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.Property;

namespace WYF.Entity
{
    /// <summary>
    /// 抽象实体模型
    /// </summary>
    public abstract class EntityType : DynamicObjectType, ISupportInitialize
    {
        [NonSerialized]
        private bool _isInitialize;
        /// <summary>
        ///字段集合
        /// </summary>
        [NonSerialized]
        protected volatile Dictionary<string, IDataEntityProperty> fields;
        /// <summary>
        /// 属性集合
        /// </summary>
        [NonSerialized]
        protected Dictionary<string, IDataEntityProperty> propindexs;
        public EntityType()
        {

        }

        public EntityType(string name) : base(name)
        {

        }
        /// <summary>
        /// 查找属性	
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns>实体属性</returns>
        public IDataEntityProperty FindProperty(string name)
        {
            CreatePropIndexs();
            return this.propindexs.GetValueOrDefault(name, null);
        }

        /// <summary>
        /// 创建实体属性关系
        /// </summary>
        public void CreatePropIndexs()
        {
            if (this.propindexs == null)
            {
                Dictionary<string, IDataEntityProperty> caseInsensitiveDic = new Dictionary<string, IDataEntityProperty>(StringComparer.OrdinalIgnoreCase);

                CreatePropIndexs((IDataEntityType)this, caseInsensitiveDic);

                this.propindexs = caseInsensitiveDic;
            }
        }

        protected void CreatePropIndexs(IDataEntityType type, Dictionary<string, IDataEntityProperty> dictionary)
        {
            foreach (IDataEntityProperty prop in type.Properties)
            {
                CreatePropIndex(type, prop, dictionary);
            }

        }
        /// <summary>
        /// 创建实体属性关系
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prop"></param>
        /// <param name="map"></param>
        private void CreatePropIndex(IDataEntityType type, IDataEntityProperty prop, Dictionary<string, IDataEntityProperty> map)
        {
            if (prop == type.PrimaryKey)
            {
                map[type.Name + "." + prop.Name] = prop;
            }
            else
            {
                map[prop.Name] = prop;
            }
            if (prop is ICollectionProperty)
            {
                CreatePropIndexs(((ICollectionProperty)prop).ItemType, map);
            }


        }

        public EntityType GetSubEntityType(ICollection<string> properties)
        {
            HashSet<string> setprops = this.GetSubEntityTypeProperties(properties);
            List<string> props = new List<string>(setprops);
            props.Sort();
            return this.CreateSubEntityType(props);
        }
        /// <summary>
        /// 根据实体属性获取一个新的实体
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        protected EntityType CreateSubEntityType(List<String> props)
        {
            EntityType eType = null;

            try
            {
                eType = (EntityType)this.Clone();
            }
            catch (Exception ex)
            {
                throw;
            }

            return eType.CreateSubEntityTypeInner(props);
        }
        /// <summary>
        /// 根据实体属性获取一个新的实体
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        EntityType CreateSubEntityTypeInner(List<string> props)
        {
            ExtractOption option = new ExtractOption();
            option.IncludeRefId = true;
            MasterBasedataProp masterProp = null;
            List<string> refMasterProps = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            foreach (string prop in props)
            {
                string[] paths = prop.Split(".");
                IDataEntityProperty p = this.FindProperty(paths[0]);
                if (p is EntryProp && paths.Length > 1)
                {

                }
                if (p != null)
                {
                    this.RegisterSubProp(option, p);
                    if (p is MasterBasedataProp && paths.Length > 1)
                    {
                        masterProp = (MasterBasedataProp)p;
                        refMasterProps.Add(prop.Substring(p.Name.Length + 1));
                    }
                }
                sb.Append(prop).Append(',');

            }
            if (masterProp != null && !refMasterProps.Any())
            {
                //此处未实现
            }
            if (sb.Length > 1)
            {
                sb = sb.Remove(sb.Length - 1, 1);

            }
            sb.Append(']');
            this.Sub(option);
            this.ExtendName = sb.Insert(0, this.Name).ToString();
            return this;
        }
        /// <summary>
        /// 剔除属性，重新构造一个新的实体
        /// </summary>
        /// <param name="option"></param>
        protected void Sub(ExtractOption option)
        {
            //
            HashSet<string> selectedProperties = option.GetExtractedProperties(this.Name);
            if (selectedProperties == null)
                selectedProperties = new HashSet<string>();
            List<IDataEntityProperty> newProperties = new List<IDataEntityProperty>();
            foreach (IDataEntityProperty property in this.Properties)
            {
                string currentPropertyName = property.Name;
                if (Exists(selectedProperties, currentPropertyName) || property == this.PrimaryKey)
                {
                    newProperties.Add(property);
                    if (property is DynamicCollectionProperty)
                    {

                        DynamicCollectionProperty colp = (DynamicCollectionProperty)property;
                        if (colp.ItemType is EntityType)
                        {
                            EntityType itemType = (EntityType)colp.ItemType;
                            itemType.Sub(option);
                        }
                        continue;

                    }
                    if (property is BasedataProp && option.IncludeRefId)
                    {
                        DynamicProperty refIdProp = this.GetProperty(((BasedataProp)property).RefIdPropName);
                        if (refIdProp == null)
                            continue;
                        newProperties.Add(refIdProp);
                        continue;
                    }

                }
            }

            ResetProperties(newProperties);
            this.propindexs = null;

        }
        private static bool Exists(HashSet<string> array, String str)
        {
            if (array != null)
                return array.Contains(str);
            return false;
        }

        private void RegisterSubProp(ExtractOption option, IDataEntityProperty p)
        {
            if (p != null)
            {
                option.Register(p.Parent.Name, new string[] { p.Name });
                if (p.Parent is EntryType)
                {
                    RegisterSubProp(option, FindProperty(p.Parent.Name));
                }
            }
        }

        public HashSet<string> GetSubEntityTypeProperties(IEnumerable<string> properties)
        {
            HashSet<string> setprops = null;
            if (properties is HashSet<string>)
            {
                setprops = (HashSet<string>)properties;
            }
            else
            {
                setprops = new HashSet<string>(16);
                foreach (string prop in properties)
                {
                    setprops.Add(prop);
                }
            }

            return setprops;
        }




        public virtual void BeginInit()
        {
            _isInitialize = true;
        }

        public virtual void EndInit()
        {
            this._isInitialize = false;
        }
    }
}
