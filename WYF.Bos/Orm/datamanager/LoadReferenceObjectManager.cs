using Antlr4.Runtime.Misc;
using WYF.Bos.algo.sql.g;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.Bos.Orm.Drivers;
using WYF.Bos.Orm.Exceptions;
using WYF.Bos.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public class LoadReferenceObjectManager
    {
        private static  List<DataEntityReferenceList> _emptyTasks = new List<DataEntityReferenceList>();
        private IDataEntityType _dt;

        private DataEntityReferenceSchema _refSchema;

        private Dictionary<String, DataEntityReferenceList> _loadTasks;
        private bool _onlyDbProperty;

        public bool OnlyDbProperty { get;set; }
        public LoadReferenceObjectManager(IDataEntityType dt, bool onlyDbProperty)
        {
            if (dt == null)
                throw new ORMArgInvalidException("100001",
                    "创建延迟读取未填充引用属性器LoadReferenceObjectManager失败，参数实体类型[IDataEntityType]不能为空！");
            this.OnlyDbProperty = onlyDbProperty;
            this._dt = dt;
            DataEntityReferenceSchema referenceSchema = new DataEntityReferenceSchema();
            this._refSchema = referenceSchema.GetReferenceSchema(this._dt, this.OnlyDbProperty);
            this._loadTasks = new Dictionary<string, DataEntityReferenceList>();
        }

        protected List<DataEntityReferenceList> GetTasks(object[] dataEntities)
        {
            if (this._refSchema == null || dataEntities == null || dataEntities.Length == 0)
                return _emptyTasks;

            Action<DataEntityWalkerEventArgs> callback = (e) =>
            {
            foreach (DataEntityReferenceSchemaItem item in this._refSchema
      .GetItemsByPropertyPath(e.PropertyStock.PropertyPath, true)) {
                String lastKey = "";
                DataEntityReferenceList list = null;
                foreach (Object dataEntity in e.DataEntities)
                {
                    Object oid = item.ReferenceOidProperty.GetValueFast(dataEntity);
                    if (!LoadReferenceObjectManager.IsDbNull(oid))
                    {
                        AutoDataEntityReference entityRef;
                        IDataEntityType refDT = item.ReferenceObjectProperty.GetComplexType(dataEntity);
                        if (refDT == null)
                            continue;
                        String taskKey = refDT.Name;
                        if (!taskKey.Equals(lastKey))
                        {
                            lastKey = taskKey;
                            list = (DataEntityReferenceList)this._loadTasks.GetValueOrDefault(taskKey);
                            if (list == null)
                            {
                                list = new DataEntityReferenceList(item.ReferenceObjectProperty.GetComplexType(dataEntity));
                                    this._loadTasks[taskKey] = list;
                            }
                        }
                        DataEntityReference reference = null;
                        if (list == null)
                            throw new Exception("loadReferenceObject error,can not find refDT task");
                        if (list.TryGet(oid, out reference))
                        {
                            entityRef = (AutoDataEntityReference)reference;
                        }
                        else
                        {
                            entityRef = new AutoDataEntityReference(oid);
                            list.Add(entityRef);
                        }
                            
                        entityRef.ReferencePropertyName = item.ReferenceObjectProperty.Name;
                        entityRef.List.Add(new Tuple<IComplexProperty, object>(item.ReferenceObjectProperty, dataEntity) );
                
                    }
                }
            }
        };

            OrmUtils.DataEntityWalker(dataEntities.ToList(), this._dt, callback, this._onlyDbProperty);
            return this._loadTasks.Values.ToList();
        }


        protected void DoTasks(List<DataEntityReferenceList> tasks)
        {
            DoTasks(tasks, new Dictionary<string, string>());
        }

        protected void DoTasks(List<DataEntityReferenceList> tasks, Dictionary<String, String> dictReferenceWhere)
        {
            if (tasks == null || tasks.Count == 0)
                return;
            foreach (DataEntityReferenceList task in tasks)
            {
                Object[] oids = task.GetNotLoadedOids();
                if (oids.Length > 0)
                {
                    Object[] dataEntities;
                    String sql = "";
                    DataEntityReference reference = null;
                    AutoDataEntityReference item = null;
                    if (task.TryGet(oids[0], out reference))
                    {
                        item = (AutoDataEntityReference)reference;
                    }
                    else
                    {
                        item = new AutoDataEntityReference(oids[0]);
                        task.Add(item);
                    }
                    if (dictReferenceWhere.Count > 0 && (
                      sql = dictReferenceWhere.GetValueOrDefault(item.ReferencePropertyName)) != null)
                    {
                        ReadWhere where = new ReadWhere(oids);
                        where.WhereSql = sql;
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

        private static void Map(DataEntityReferenceList task, Object[] dataEntities)
        {
            if (dataEntities == null)
                return;
            ISimpleProperty pk = task.DataEntityType.PrimaryKey;

            DataEntityReference refItem = null;
            foreach (Object item in dataEntities)
            {
                Object oid = pk.GetValue(item);
                bool tempVar = task.TryGet(oid, out refItem);
                if (tempVar)
                    refItem.DataEntity = item;
            }
        }
        protected Object[] Read(IDataEntityType dt, Object[] oids)
        {
            return (new impl.DataManager(dt)).Read(oids);
        }

        protected Object[] Read(IDataEntityType dt, ReadWhere where)
        {
            return (new WYF.Bos.Orm.impl.DataManager(dt)).Read(where);
        }
        private static bool IsDbNull(object oid)
        {
            if (oid == null)
                return true;
            if (oid is int)
                return ((int)oid == 0);
            if (oid is long)
                return ((long)oid == 0);
            if (oid is string)
                return string.IsNullOrWhiteSpace((string)oid);
            return false;
        }

        private   class AutoDataEntityReference : DataEntityReference
        {
         public String ReferencePropertyName { get; set; }

         public ArrayList<Tuple<IComplexProperty, Object>> List { get; set; }

   

 

        public AutoDataEntityReference(Object oid):base(oid)
        {
                this.List = new ArrayList<Tuple<IComplexProperty, object>>();
        }
         public new  Object DataEntity 
            { 
                get 

                { 
                    return base.DataEntity; 
                }
                set
                {
                    if (base.DataEntity!=value)
                    {
                        base.DataEntity = value;
                        IDataEntityType dt = ((IComplexProperty)(this.List[0].Item1)).ComplexType;
                        bool bShare = (dt == null) ? true : ((dt.CacheType == DataEntityCacheType.Share));
                        if (bShare)
                        {
                            foreach (Tuple<IComplexProperty, Object> item in this.List)
                            {
                                IComplexProperty prop = (IComplexProperty)item.Item1;
                                if (value == null && !prop.IsDbIgnore)
                                    continue;
                                prop.LoadValue(item.Item2, value);
                            }
                        }
                        else
                        {
                            foreach (Tuple<IComplexProperty, Object> item in this.List)
                                ((IComplexProperty)item.Item1).LoadValue(item.Item2, OrmUtils.Clone((IDataEntityBase)value, false, false));
                        }
                    }
                }
            }
    

   
    }
}
}
