
using WYF.DataEntity.Entity;

using System.Collections.Concurrent;

using System.ComponentModel;

using System.Reflection;



namespace WYF.DataEntity.Metadata.Clr
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public sealed class DataEntityType : IDataEntityType
    {

        private static ConcurrentDictionary<Type, DataEntityType> _dtCache = new ConcurrentDictionary<Type, DataEntityType>();
        private bool loaded = false;
        private IDataEntityType parent;
        private Type type;
        private string name;
        private string tableName;
        private bool isDbIgnore;
        //private IGetDataEntityStateHandler getDataEntityStateHandler;
        private static object _lockObject = new object();
        private Func<object, DataEntityState> getDataEntityStateHandler;

        private DataEntityCacheType cacheType;
        private DataEntityPropertyCollection properties;
        private ISimpleProperty primaryKey;
        private DataEntityTypeFlag flag;
        private List<ISimpleProperty> sortProperties;
        private string dbRouteKey;
        private object dataEntityReferenceSchema;

        public bool IsLoaded 
        { 
            get { return loaded; }
            set { loaded = value; }
        }

        public Type DataEntityClass { get { return type; } }
        public DataEntityType(Type type)
        {
            this.cacheType = DataEntityCacheType.Share;
            this.flag = DataEntityTypeFlag.Class;
            this.type = type;
        }

        public PkSnapshotSet GetPkSnapshot(object dataEntity)
        {
            return this.GetDataEntityState(dataEntity).PkSnapshotSet;
        }

        private void Initialize()
        {
            this.flag = this.GetDataEntityTypeFlag();
            DataEntityTypeAttribute typeAtt = (DataEntityTypeAttribute)this.type.GetCustomAttributes(typeof(DataEntityTypeAttribute), true).FirstOrDefault();
            this.name = typeAtt != null && !string.IsNullOrWhiteSpace(typeAtt.Name) ? typeAtt.Name : this.type.Name;
            this.tableName = typeAtt != null && !string.IsNullOrWhiteSpace(typeAtt.TableName) ? typeAtt.TableName : "";
            if (typeAtt != null)
            {
                this.isDbIgnore = typeAtt.IsDbIgnore;
                this.dbRouteKey = typeAtt.DbRouteKey;
            }
            if (this.flag == DataEntityTypeFlag.Primitive)
            {
                this.properties = new DataEntityPropertyCollection(new List<IDataEntityProperty>(), this);
            }
            else
            {

                PropertyInfo[] propertyInfos = this.type.GetProperties();
                List<IDataEntityProperty> list = new List<IDataEntityProperty>(propertyInfos.Length);
                int ordinal = 0;
                foreach (PropertyInfo pitem in propertyInfos)
                {
                    if (pitem.IsDefined(typeof(SimplePropertyAttribute)))
                    {
                        DataEntityProperty property = new SimpleProperty(pitem, ordinal++);

                        property.Parent = this;
                        list.Add(property);
                    }
                    else if (pitem.IsDefined(typeof(ComplexPropertyAttribute)))
                    {
                        DataEntityProperty property = new ComplexProperty(pitem, ordinal++);
                        property.Parent = this;
                        list.Add(property);
                    }
                    else if (pitem.IsDefined(typeof(CollectionPropertyAttribute)))
                    {
                        CollectionProperty cproperty = new CollectionProperty(pitem, ordinal++);
                        cproperty.Parent = this;
                        if (cproperty.ItemType is DataEntityType) {
                            ((DataEntityType)cproperty.ItemType).parent = this;
                        }

                        list.Add(cproperty);
                    }
                }
                list.TrimExcess();
                this.properties = new DataEntityPropertyCollection(list, this);
            }



        }
        /// <summary>
        /// 获取实体类型的特征（抽象类  一般类 还是接口等）
        /// </summary>
        /// <returns></returns>
        private DataEntityTypeFlag GetDataEntityTypeFlag()
        {
            if (!this.type.IsPrimitive && !this.type.IsEnum)
            {
                if (this.type.IsAbstract)
                {
                    return DataEntityTypeFlag.Abstract;
                }
                else
                {
                    return this.type.IsInterface ? DataEntityTypeFlag.Interface : DataEntityTypeFlag.Class;
                }
            }
            else
            {
                return DataEntityTypeFlag.Primitive;
            }
        }

        public ISimpleProperty PrimaryKey { get { return this.primaryKey; } }

        public List<ISimpleProperty> SortProperties
        {
            get
            {
                if (this.sortProperties == null)
                    this.sortProperties = new List<ISimpleProperty> ();
                return this.sortProperties;
            }
        }

        public DataEntityCacheType CacheType
        {
            get { return this.cacheType; }
            set { this.cacheType = value; }
        }

        public string DBRouteKey
        {
            get
            {
                return this.dbRouteKey;
            }

            set { this.dbRouteKey = value; }
        }

        public DataEntityTypeFlag Flag
        {
            get { return this.flag; }
        }

        public DataEntityPropertyCollection Properties => this.properties;

        public string Name => this.name;

        public string Alias => this.tableName;

        public bool IsDbIgnore => this.isDbIgnore;

        public object CreateInstance()
        {
            return Activator.CreateInstance(this.type);
        }

        public object CreateInstance(bool isQueryObj)
        {
            return CreateInstance();
        }

        public List<IDataEntityProperty> GetJsonSerializerProperties()
        {
            return this.Properties.ToList();
        }

        public IDataEntityType Parent 
        { 
            get {  return this.parent; }
            set { this.parent = value; }
        }

        public HashSet<string> FullIndexFields { get; set; }
        public object DataEntityReferenceSchema {
            get { return this.dataEntityReferenceSchema; }
            set { this.dataEntityReferenceSchema = value; } 
        }

        public object GetParent(object currentObject)
        {
            return null;
        }
        /// <summary>
        /// 返回某个实体数据是否已经发生了变更
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public bool IsDirty(object dataEntity)
        {
            return GetDataEntityState(dataEntity).DataEntityDirty;
        }


        public void SetPkSnapshot(object dataEntity, PkSnapshotSet pkSnapshotSet)
        {
            this.GetDataEntityState(dataEntity).PkSnapshotSet = pkSnapshotSet;
        }


        /// <summary>
        /// 根据实体数据返回是否为空
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public bool IsEmpty(Object dataEntity)
        {
            if (dataEntity == null)
                return true;
            foreach (IDataEntityProperty prop in Properties)
            {
                if (!prop.IsEmpty(dataEntity))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 设置一个实体是从数据库加载而来，并且清除脏标志，当读取或保存完毕后，调用此方法
        /// </summary>
        /// <param name="dataEntity"></param>
        public void SetFromDatabase(object dataEntity)
        {
            GetDataEntityState(dataEntity).IsFromDatabase = true;
        }
        public DataEntityState GetDataEntityState(object dataEntity)
        {
            if (this.getDataEntityStateHandler == null)
            {
                
                lock (this)
                {
                    if (this.getDataEntityStateHandler == null)
                    {
                        this.getDataEntityStateHandler = CreateGetDataEntityStateHandler(this);
                    }
                }
            }

             return  this.getDataEntityStateHandler(dataEntity);


        }

        private Func<object, DataEntityState> CreateGetDataEntityStateHandler(DataEntityType dt)
        {
            Type type=  dt.DataEntityClass;
            PropertyInfo property = type.GetProperty("DataEntityState", typeof(DataEntityState));
            if (property != null)
            {
                DataEntityProperty p2 = new DataEntityProperty(property, -1);
                return dataEntity =>(DataEntityState)p2.GetValue(dataEntity);

            }

            return dataEntity => new AlwaysDirtyDataEntityState(dt);
        }

        public void SetFromDatabase(object dataEntity, bool clearDirty)
        {
            GetDataEntityState(dataEntity).SetFromDatabase(true, clearDirty);
        }

        public static IDataEntityType GetDataEntityType(Type type)
        {

            if (type == null) {
                throw new Exception("根据Type类型获取实体类型DataEntityType失败，Type类型不能为空！");

            }

            DataEntityType dataEntityType= _dtCache.GetOrAdd(type, (type) => 
            {
                DataEntityType value = new DataEntityType(type);
                value.Initialize();
                value.IsLoaded=true;
                return value;

            });
            return dataEntityType;
        }

        public void SetDirty(object dataEntity, bool newValue)
        {
            this.GetDataEntityState(dataEntity).SetDirty(newValue);
        }

        public IEnumerable<IDataEntityProperty> GetDirtyProperties(object dataEntity)
        {
            return this.GetDataEntityState(dataEntity).GetDirtyProperties();
        }

        public IEnumerable<IDataEntityProperty> GetDirtyProperties(object dataEntity, bool includehasDefualt)
        {
            return this.GetDataEntityState(dataEntity).GetDirtyProperties(includehasDefualt);
        }

 

        private sealed class AlwaysDirtyDataEntityState : DataEntityState
        {

            private DataEntityType _dt;


            public AlwaysDirtyDataEntityState(DataEntityType dt)
            {
                this._dt = dt;
            }

            public override IEnumerable<IDataEntityProperty> GetDirtyProperties()
            {
                return this._dt.Properties;
            }

            public override IEnumerable<IDataEntityProperty> GetDirtyProperties(bool includehasDefualt)
            {
                return this._dt.Properties;
            }

            public override void SetDirty(bool newValue)
            {
            }

            public override void SetPropertyChanged(PropertyChangedEventArgs e)
            {
            }

            public override void SetDirtyFlags(int[] values)
            {
                throw new NotImplementedException();
            }

            public override int[] GetDirtyFlags()
            {
                throw new NotImplementedException();
            }

            public override bool DataEntityDirty
            {
                get
                {
                    return true;
                }
            }
        }
    }
}
