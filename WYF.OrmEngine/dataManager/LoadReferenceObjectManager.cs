using System.Collections.Concurrent;
using System.Diagnostics;
using WYF.Bos.DataEntity;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Utils;
using WYF.OrmEngine.dataManager;
using WYF.OrmEngine.Impl;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 管理和加载数据实体中的引用对象。它通过延迟加载（懒加载）的方式来优化性能，即在需要时才加载引用的对象，而不是一次性加载所有相关数据
    /// </summary>
    public class LoadReferenceObjectManager
    {
        private IDataEntityType _dt;
        private DataEntityReferenceSchema _refSchema;
        private ConcurrentDictionary<string, DataEntityReferenceList> _loadTasks;
        private bool _onlyDbProperty;

        // 构造函数，初始化引用管理器
        protected LoadReferenceObjectManager(IDataEntityType dt)
            : this(dt, true) { }

        // 构造函数，初始化引用管理器并指定是否只考虑数据库属性
        public LoadReferenceObjectManager(IDataEntityType dt, bool onlyDbProperty)
        {
            if (dt == null)
                throw new ORMArgInvalidException("100001", "创建延迟读取未填充引用属性器LoadReferenceObjectManager失败，参数实体类型[IDataEntityType]不能为空！");
            _onlyDbProperty = onlyDbProperty;
            _dt = dt;
            DataEntityReferenceSchema referenceSchema = new DataEntityReferenceSchema();
            _refSchema = referenceSchema.GetReferenceSchema(_dt, GetOnlyDbProperty());
            _loadTasks = new ConcurrentDictionary<string, DataEntityReferenceList>();
        }

        // 获取是否只考虑数据库属性
        public bool GetOnlyDbProperty()
        {
            return _onlyDbProperty;
        }

        // 加载引用对象
        public void Load(object[] dataEntities)
        {
            DoTasks(GetTasks(dataEntities));
        }

        // 加载引用对象，并使用自定义的where条件
        public void Load(object[] dataEntities, Dictionary<string, string> dictReferenceWhere)
        {
            DoTasks(GetTasks(dataEntities), dictReferenceWhere);
        }

        // 空任务列表
        private static readonly List<DataEntityReferenceList> _emptyTasks = new List<DataEntityReferenceList>();

        // 获取加载任务
        protected virtual ICollection<DataEntityReferenceList> GetTasks(object[] dataEntities)
        {
            if (_refSchema == null || dataEntities == null || dataEntities.Length == 0)
                return _emptyTasks;

            Action<DataEntityWalkerEventArgs> listner = (e) =>
            {
                foreach (var item in _refSchema.GetItemsByPropertyPath(e.PropertyStock.PropertyPath, true))
                {
                    string lastKey = "";
                    DataEntityReferenceList list = null;
                    foreach (var dataEntity in e.DataEntities)
                    {
                        var oid = item.ReferenceOidProperty.GetValueFast(dataEntity);
                        if (!IsDbNull(oid))
                        {
                            AutoDataEntityReference entityRef;
                            var refDT = item.ReferenceObjectProperty.GetComplexType(dataEntity);
                            if (refDT == null) continue;

                            string taskKey = refDT.Name;
                            if (taskKey != lastKey)
                            {
                                lastKey = taskKey;
                                if (!_loadTasks.TryGetValue(taskKey, out list))
                                {
                                    list = new DataEntityReferenceList(item.ReferenceObjectProperty.GetComplexType(dataEntity));
                                    _loadTasks[taskKey] = list;
                                }
                            }

                            DataEntityReference? ref_obj = null;
                            if (list.TryGet(oid, out ref_obj))
                            {
                                entityRef = (AutoDataEntityReference)ref_obj;
                            }
                            else
                            {
                                entityRef = new AutoDataEntityReference(oid);
                                list.Add(entityRef);
                            }

                            entityRef.ReferencePropertyName = item.ReferenceObjectProperty.Name;
                            entityRef.List.Add(Tuple.Create(item.ReferenceObjectProperty, dataEntity));
                        }
                    }
                }
            };

            OrmUtils.DataEntityWalker(dataEntities.ToList(), _dt, listner, _onlyDbProperty);
            return _loadTasks.Values;
        }

        // 判断OID是否为空
        private static bool IsDbNull(object oid)
        {
            if (oid == null) return true;
            if (oid is int) return (int)oid == 0;
            if (oid is long) return (long)oid == 0L;
            if (oid is string) return string.IsNullOrEmpty(oid.ToNullString());
            return false;
        }

        // 自动数据实体引用类
        private sealed class AutoDataEntityReference : DataEntityReference
        {
            public string ReferencePropertyName { get; set; }
            public List<Tuple<IComplexProperty, object>> List { get; } = new List<Tuple<IComplexProperty, object>>();

            public AutoDataEntityReference(object oid) : base(oid) { }

            public override object DataEntity 
            { 
                get {  return base.DataEntity; }

                set {

                    if (base.DataEntity != value)
                    {
                        base.SetDataEntity(value);
                        var dt = ((IComplexProperty)List[0].Item1).ComplexType;
                        bool bShare = dt == null ? true : (dt.CacheType == DataEntityCacheType.Share);
                        if (bShare)
                        {
                            foreach (var item in List)
                            {
                                var prop = (IComplexProperty)item.Item1;
                                if (value == null && !prop.IsDbIgnore) continue;
                                prop.LoadValue(item.Item2, value);
                            }
                        }
                        else
                        {
                            foreach (var item in List)
                            {
                                ((IComplexProperty)item.Item1).LoadValue(item.Item2, OrmUtils.Clone((IDataEntityBase)value, false, false));
                            }
                        }
                    }
                }
            }
           

        
        }

        // 执行加载任务
        protected void DoTasks(ICollection<DataEntityReferenceList> tasks)
        {
            DoTasks(tasks, new Dictionary<string, string>());
        }

        // 执行加载任务，并使用自定义的where条件
        protected void DoTasks(ICollection<DataEntityReferenceList> tasks, Dictionary<string, string> dictReferenceWhere)
        {
            if (tasks == null || tasks.Count == 0) return;

            foreach (var task in tasks)
            {
                var oids = task.GetNotLoadedOids();
                if (oids.Length > 0)
                {
                    object[] dataEntities;
                    string sql = "";
                    DataEntityReference? ref_obj = null;
                    AutoDataEntityReference item = null;
                    if (task.TryGet(oids[0], out ref_obj))
                    {
                        item = (AutoDataEntityReference)ref_obj;
                    }
                    else
                    {
                        item = new AutoDataEntityReference(oids[0]);
                        task.Add(item);
                    }

                    if (dictReferenceWhere.Count > 0 && (sql = dictReferenceWhere[item.ReferencePropertyName]) != null)
                    {
                        var where = new ReadWhere(oids) { WhereSql = sql };
                        dataEntities = Read(task.DataEntityType, where);
                    }
                    else
                    {
                        dataEntities = Read(task.DataEntityType, oids);
                    }

                    Map(task, dataEntities);
                }
            }
        }

        // 映射加载的数据到引用列表
        private static void Map(DataEntityReferenceList task, object[] dataEntities)
        {
            if (dataEntities == null) return;

            var pk = task.DataEntityType.PrimaryKey;
            Debug.Assert(pk != null);

            DataEntityReference refItem = null;
            foreach (var item in dataEntities)
            {
                var oid = pk.GetValue(item);
                DataEntityReference tempRef_refItem = null;
                if (task.TryGet(oid, out tempRef_refItem))
                {
                    refItem = (DataEntityReference)tempRef_refItem;
                    refItem.SetDataEntity(item);
                }
            }
        }

        // 读取数据
        protected virtual object[] Read(IDataEntityType dt, object[] oids)
        {
            return (new DataManager(dt)).Read(oids);
        }

        // 读取数据，并使用自定义的where条件
        protected virtual object[] Read(IDataEntityType dt, ReadWhere where)
        {
            return (new DataManager(dt)).Read(where);
        }
    }
}