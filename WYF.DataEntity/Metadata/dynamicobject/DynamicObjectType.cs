
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Clr;
using System;
using System.Collections.Generic;

namespace WYF.DataEntity.Metadata.Dynamicobject
{
    /// <summary>
    /// DynamicObjectType允许在运行时动态构建一个实体类型，而不需要在设计时预先定义好实体类型。
    /// 
    /// </summary>
    [Serializable]
    public class DynamicObjectType : DynamicMetadata, IDataEntityType, ICloneable
    {

        private String _tableName;
        private String _name;
        private String displayName;
        private String _extendName;
        private String dbRouteKey;
        public Type _clrType;
        private List<ISimpleProperty> _sortProperties;
        private ISimpleProperty _primaryKey;
        private DynamicPropertyCollection _properties;
        private object[] _attributes;
        private DataEntityTypeFlag _flag;
        private DataEntityCacheType _cacheType;
        private DynamicObjectType _baseType;
        IDataEntityType _parent;

        private  object _dataEntityReferenceSchema;
        public DynamicObjectType()
        {
            this._cacheType = DataEntityCacheType.Share;
            this.dbRouteKey = "";
            this._flag = DataEntityTypeFlag.Class;
            this._properties = new DynamicPropertyCollection(new List<IDataEntityProperty>(), this);
        }


        public object DataEntityReferenceSchema
        {
            get { return _dataEntityReferenceSchema; }
            set { this._dataEntityReferenceSchema = value; }
        }

        /// <summary>
        /// 实体类型的名称
        /// </summary>
        [SimpleProperty]
        public override string Name
        {
            get { return _name; }
            set { this._name = value; }
        }
        [SimpleProperty]
        public string DisplayName
        {
            get { return displayName; }
             set { this.displayName = value; }
        }
        /// <summary>
        /// 实体表名
        /// </summary>
        [SimpleProperty]
        public override string Alias 
        { 
            get { return _tableName; }
            set { this._tableName = value; }
        }



        /// <summary>
        /// 扩展名称；类型名称与各属性名合并组成的字符串； 以此快速比对两个类型是否一致，作为缓存Region标识
        /// </summary>
        [SimpleProperty]
        public string ExtendName
        {
            get {
                return string.IsNullOrEmpty(_extendName) ? _name : _extendName;
            }
             set { this._extendName = value; }
        }
        /// <summary>
        /// 主键  this._primaryKey;
        /// </summary>
        public ISimpleProperty PrimaryKey { 
            get
            {
                if (this._primaryKey == null)
                {
                    foreach (IDataEntityProperty prop in this._properties)
                    {
                        if (prop is ISimpleProperty simpleProp && simpleProp.IsPrimaryKey)
                        {
                            this._primaryKey = simpleProp;
                            break;
                        }
                    }
                }

                return this._primaryKey;
            } 
        }
        
        public Type ClrType { 
           get { 
            
                if (_clrType == null)
                {
                    return typeof(DynamicObject);
                }
                return this._clrType;
            } 
        }

      


        public List<IDataEntityProperty> jsonSerializerProperties { get; set; }

        /// <summary>
        /// 该实体类型对应的父实体
        /// </summary>
        public IDataEntityType Parent 
        { 
            get { return this._parent; } 
            set { this._parent = value; } 
        }


        /// <summary>
        /// 定义了实体类型的特征，可选值包括：Class(缺省)、Abstract、Sealed、Interface
        /// </summary>
        public DataEntityTypeFlag Flag => this._flag;

        /// <summary>
        /// 实体缓存的类型，可选值包括： 共享型（缺省），隔离型
        /// </summary>
        public DataEntityCacheType CacheType => this._cacheType;

        public List<ISimpleProperty> SortProperties { 
            get
            {
                if (this._sortProperties == null)
                    this._sortProperties = new List<ISimpleProperty> ();
                return this._sortProperties;
            } 
        }
        /// <summary>
        /// 实体对应路由
        /// </summary>
        [SimpleProperty]
        public string DBRouteKey { 
            
            get { return this.dbRouteKey; }
            set { this.dbRouteKey = value; }
        }


        //[CollectionProperty(CollectionItemPropertyType = typeof(DynamicProperty))]
        //public DynamicPropertyCollection Properties => this._properties;

        [CollectionProperty(CollectionItemPropertyType = typeof(DynamicProperty))]
        public DataEntityPropertyCollection Properties
        {
            get
            {
                return _properties;
            }
        }

        public DynamicProperty GetProperty(string propertyName)
        {
            _properties.TryGetValue(propertyName, out IDataEntityProperty property);
            return (DynamicProperty)property;
        }


        public HashSet<string> FullIndexFields { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //[SimpleProperty]
        //public  int CacheTypeId { get { return CacheType.GetValue<int>(); } set { this.CacheType= CacheType.get }

        public DynamicObjectType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception($"设置动态实体类型{name}的名称失败,名称不规范");
            }
            this._cacheType = DataEntityCacheType.Share;
            this.dbRouteKey = "";
            this._flag = DataEntityTypeFlag.Class;
            this._name = name;
            this._properties = new DynamicPropertyCollection(new List <IDataEntityProperty>(), this);
        }
        void IDataEntityType.SetDirty(object dataEntity, bool newValue)
        {
            DynamicObjectType.ConvertToDynamicObject(dataEntity).DataEntityState.SetDirty(newValue);
        }

        PkSnapshotSet IDataEntityType.GetPkSnapshot(object dataEntity)
        {
            return DynamicObjectType.ConvertToDynamicObject(dataEntity).DataEntityState.PkSnapshotSet;
        }
        IEnumerable<IDataEntityProperty> IDataEntityType.GetDirtyProperties(object dataEntity)
        {
            return DynamicObjectType.ConvertToDynamicObject(dataEntity).DataEntityState.GetDirtyProperties();
        }
        IEnumerable<IDataEntityProperty> IDataEntityType.GetDirtyProperties(object dataEntity, bool includehasDefualt)
        {
            return ConvertToDynamicObject(dataEntity).DataEntityState.GetDirtyProperties(includehasDefualt);
        }
        void IDataEntityType.SetPkSnapshot(object dataEntity, PkSnapshotSet pkSnapshotSet)
        {
            DynamicObjectType.ConvertToDynamicObject(dataEntity).DataEntityState.PkSnapshotSet = pkSnapshotSet;
        }
        public DynamicProperty RegisterProperty(string name, Type propertyType, object defaultValue, bool isReadonly)
        {
            if (propertyType == null) throw new Exception($"对实体类型{this.Name}注册简单属性{name}失败,属性类型propertyType不能为空！");
                
            if (propertyType.IsPrimitive && defaultValue == null)
            {
                if (propertyType == typeof(bool))
                {
                    defaultValue = false;
                }
                else
                {
                    defaultValue = Convert.ChangeType(0, propertyType);
                }
            }
            DynamicProperty property = new DynamicProperty(name, propertyType, defaultValue, isReadonly);
            AddProperty(property);
            return property;
        }
        /// <summary>
        /// 注册一个简单属性，此属性参与ORM的处理 简单属性对应数据库中最基本的类型,包括(整型、字符、浮点型、日期等)
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public  DynamicProperty RegisterSimpleProperty(DynamicSimpleProperty property)
        {
            AddProperty(property);
            return property;
        }

        public  DynamicCollectionProperty RegisterCollectionProperty(DynamicCollectionProperty property)
        {
            if (property.DynamicCollectionItemPropertyType == null) throw new Exception($"对实体类型{this.Name}注册复杂属性{property.Name}失败,属性类型itemDataEntityType不能为空！");
            AddProperty(property);
            return property;
        }

        public void AddProperty(DynamicProperty property)
        {
          
            this._properties.Add(property);
        }


        public object Clone()
        {
            DynamicObjectType cloneDT = (DynamicObjectType)base.Clone();
            cloneDT.DataEntityReferenceSchema = null;

            List<IDataEntityProperty> cloneProperties = new List<IDataEntityProperty>(this.Properties);
            foreach (IDataEntityProperty property in cloneProperties)
            {
                DynamicProperty cloneProp = (DynamicProperty)property.Clone();
                cloneProp.Parent = cloneDT;
                // No need to add cloneProp to a newProperties list as it is already part of cloneProperties  
            }

            cloneDT.ResetProperties(cloneProperties);

            foreach (IDataEntityProperty property in cloneDT.Properties)
            {
                DynamicProperty cloneProp = (DynamicProperty)property;
                cloneProp.AfterClone();
            }

            cloneDT.AfterClone();
            return cloneDT;
        }

        protected void ResetProperties(List<IDataEntityProperty> newProperties)
        {
            this.jsonSerializerProperties = null;
            this._properties = new DynamicPropertyCollection(newProperties, this);
            for (int i = 0; i < newProperties.Count; i++)
            {
                DynamicProperty p = (DynamicProperty)newProperties[i];
                p.Ordinal = i;
            }

    

        }


        protected void AfterClone()
        {
            this._primaryKey = null;
            //this.CreatedByProperty = null;
            //this.createTimeProperty = null;
            //this.modifierProperty = null;
            //this.modifyTimeProperty = null;
            //this.unmodifiable = false;
        }

        public object CreateInstance()
        {
            return new DynamicObject(this);
        }

        public object CreateInstance(bool isQueryObj)
        {
            return new DynamicObject(this, true);
        }

        public bool IsDirty(object dataEntity)
        {
            throw new NotImplementedException();
        }


        public bool IsEmpty(object dataEntity)
        {
            if (dataEntity == null)
                return true;
            foreach (IDataEntityProperty prop in this.Properties)
            {
                if (!prop.IsEmpty(dataEntity))
                    return false;
            }
            return true;
        }
       

        public void SetFromDatabase(object dataEntity)
        {
            DynamicObject dynamicObject = ConvertToDynamicObject(dataEntity);
            dynamicObject.DataEntityState.IsFromDatabase = true;
        }

        public void SetFromDatabase(object dataEntity, bool clearDirty)
        {
            throw new NotImplementedException();
        }
   
        private static DynamicObject ConvertToDynamicObject(Object dataEntity)
        {
            if (dataEntity == null) throw new Exception("转换对象为动态实体失败，要转换的对象不能为空！");
                
            if (!(dataEntity is DynamicObject)) throw new Exception($"转换对象{dataEntity.ToString()}为动态实体失败，该对象必须是DynamicObject类型！");
             
            return (DynamicObject)dataEntity;
        }

        public object GetParent(object currentObject)
        {
            if (currentObject is DynamicObject)
            {
                return ((DynamicObject)currentObject).Parent;
            }
            return null;
        }

        public List<IDataEntityProperty> GetJsonSerializerProperties()
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            if (inherit)
            {
                List<object> list = new List<object>();
                DynamicObjectType dynamicObjectType = this;
                while (dynamicObjectType != null)
                {
                    if (dynamicObjectType._attributes != null)
                    {
                        list.AddRange(dynamicObjectType._attributes);
                    }
                    dynamicObjectType = dynamicObjectType._baseType;
                }
                return list.ToArray();
            }
            if (this._attributes == null)
            {
                return DynamicMetadata.EmptyAttributes;
            }
            return this._attributes;
        }
    }
}
