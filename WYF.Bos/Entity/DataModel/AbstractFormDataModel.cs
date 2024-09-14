using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Entity.DataModel.Events;
using WYF.Bos.Entity.property;
using WYF.Bos.Form.control.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    public class AbstractFormDataModel : IDataModel
    {

        private Dictionary<string, object> entryStates;
        private Dictionary<string, object> contextVariable;
        private  int initCounter;
        private string permissionItem;
        private bool cacheIsDirty;
        private DirtyManager dirtyManager;
        private bool onSetValue;
        protected ModelEventProxy modelEventProxy;
        private SetValueCallManager<SetValueCallInfo> callManager;
        private PropChangedContainer propChangedContainer;

        protected DynamicObject dataEntity;
        protected IDataModelRepository repository;
        private string entityName;
        private string appId;
        protected string pageId;
        private Dictionary<Type, object> services;

        private MainEntityType mainEntityType;
   
        private AbstractFormDataModel()
        {
            this.entryStates = new Dictionary<string, object>();
            this.contextVariable = new Dictionary<string, object>();
            this.initCounter = 0;
            this.permissionItem = "47156aff000000ac";
            this.cacheIsDirty = false;
            this.onSetValue = false;
            this.callManager = new SetValueCallManager<SetValueCallInfo>();
            this.dirtyManager = new DirtyManager(this);
            this.modelEventProxy = new ModelEventProxy();
            this.propChangedContainer = new DefaultPropChangedContainer(this, this.modelEventProxy);
        }


        public AbstractFormDataModel(string entityName, string pageId, Dictionary<Type, object> services):this() 
        {
            this.entityName = entityName;
            this.pageId = pageId;
            this.services = services;
            this.services[typeof(ModelEventProxy)] = this.modelEventProxy;
            this.services[typeof(PropChangedContainer)] = this.propChangedContainer;
        }
        public AbstractFormDataModel(string entityName, string pageId, Dictionary<Type, object> services, string appId, string permissionItem) : this()
        {
            this.entityName = entityName;
            this.pageId = pageId;
            this.services = services;
            this.appId = appId;
            this.permissionItem = permissionItem;
            this.services[typeof(ModelEventProxy)] = this.modelEventProxy;
            this.services[typeof(PropChangedContainer)] = this.propChangedContainer;
        }

        /// <summary>
        /// 创建空的数据包
        /// 所谓的空白数据包，实际上并不是完全空白，各个字段已经设置了默认值；单据体也已经创建了默认行
        /// </summary>
        /// <returns></returns>
        public object CreateNewData()
        {
            return this.CreateNewData(null);
        }


        public object CreateNewData(DynamicObject newObject)
        {
            bool fireAfterCreateNewDataEvent = true;
            if (newObject != null)
            {
                this.dataEntity = newObject;
                this.repository = new DataModelLocalRepository(this, this.dataEntity);

            }
            else
            {
                BizDataEventArgs e = new BizDataEventArgs();
                e.IsExecuteRule = true;
                e.FireAfterCreateNewData = true;
                this.entryStates = new Dictionary<string, object>();
                //插件可在此事件，自行构建模型数据包，后续平台将把插件构建的数据包显示在界面上，如果插件不处理此事件，平台会自行构建空白的模型数据包，并给每个字段填好默认值
                this.modelEventProxy.FireCreateNewData(e);
                if (e.DataEntity!=null)
                {
                    this.dataEntity = (DynamicObject)e.DataEntity;
                    this.repository = new DataModelLocalRepository(this, this.dataEntity);
                }
                else
                {
                    this.dataEntity = this.NewDataEntity();
                    this.repository = new DataModelLocalRepository(this, this.dataEntity);
                    this.CreateDefaultEntity();
                    this.dirtyManager.ClearDirty();
                }
            }

            this.modelEventProxy.FireAfterCreateNewData(new EventObject(this));
            SetDataChanged(false);
            return this.dataEntity;
        }

        public void SetDataChanged(bool value)
        {
            this.dirtyManager.SetBizChanged(value);
            if (!value)
                GetDataEntity().DataEntityState.RemovedItems = false;
        }
        private DynamicObject CreateDefaultEntity()
        {
            MainEntityType entityType = this.GetDataEntityType();
            DataEntityPropertyCollection propertyCollection = entityType.Properties;
         
            foreach (var prop in propertyCollection)
            {
                if (prop is IFieldHandle&& !((IFieldHandle)prop).IsSysField)
                {
                    ((IFieldHandle)prop).ApplyDefaultValue(this, this.dataEntity, -1);

                }
              
            }

            foreach (var cp in entityType.Properties.GetCollectionProperties(false))
            {
                if (cp is EntryProp){
                    EntryProp entryProp = (EntryProp)cp;
                    int rows = entryProp.DefaultRows;
                    if (rows>0)
                    {
                        BatchCreateNewEntryRow(entryProp.Name, rows);
                    }
                }
            }


            return this.dataEntity;

        }

        private int[] BatchCreateNewEntryRow(string name, int rows)
        {
            return new int[] { 1 };
         
            
        }
        public T GetService<T>()
        {
            return (T)this.services.GetValueOrDefault(typeof(T));
        }
        protected DynamicObject NewDataEntity()
        {
            return (DynamicObject)this.GetDataEntityType().CreateInstance();
        }

        public MainEntityType GetDataEntityType()
        {
            return this.GetMainEntityType();
        }

        private MainEntityType GetMainEntityType()
        {
            if (this.mainEntityType == null)
            {
                if (MainEntityType.Empty.Name== this.entityName)
                {
                    this.mainEntityType = MainEntityType.Empty;
                }
                else
                {
                    this.mainEntityType = EntityMetadataCache.GetDataEntityType(this.entityName);
                }
            }

            GetEntityTypeEventArgs e = new GetEntityTypeEventArgs(this.mainEntityType);

            this.modelEventProxy.FireGetEntityType(e);
            if (e.NewEntityType != null)
            {
                this.mainEntityType = e.NewEntityType;
            }

            return this.mainEntityType;

        }

        public void BeginInit()
        {
            Interlocked.Increment(ref initCounter);
        }

        public void EndInit()
        {
            Interlocked.Decrement(ref initCounter);
     
        }

        public void PutContextVariable(string name, object value)
        {
            this.contextVariable[name] = value;
        }

        public DynamicObject GetDataEntity()
        {
            return GetDataEntity(false);
       
        }
        /// <summary>
        /// 获取根数据包，可以指定在缓存情况是否含分录
        /// </summary>
        /// <param name="includeEntry">是否包含分录，设置为true会从缓存中恢复所有分录行放在根数据包中</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DynamicObject GetDataEntity(bool includeEntry)
        {
            //if (includeEntry && GetRepository() is DataModelCacheRepository) {

            //    this.dataEntity = GetRepository().GetAll();
            //    this.repository = null;
            //    return this.dataEntity;
            //}

            return this.GetRepository().GetRootEntity();
        }
        /// <summary>
        /// 获取字段在主实体中对应的属性对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDataEntityProperty GetProperty(string name)
        {
            MainEntityType et = GetMainEntityType();
            return et.FindProperty(name.ToLower());
        }

        protected IDataModelRepository GetRepository()
        {
            if (this.repository == null)
            {
                if (this.dataEntity == null)
                {
                    //this.repository = new DataModelCacheRepository(this);
                }
                else
                {
                    this.repository = new DataModelLocalRepository(this, this.dataEntity);
                }
            }

            return this.repository;
        }
        /// <summary>
        /// 获取上下文变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetContextVariable(string name)
        {
            var result= this.contextVariable.GetValueOrDefault(name,null);
        
            return result;


        }

        public void AddDataModelListener(IDataModelListener listener)
        {
            this.modelEventProxy.AddDataModelListener(listener);
        }

        public void AddDataModelChangeListener(IDataModelChangeListener listener)
        {
            this.modelEventProxy.AddDataModelChangeListener(listener);
        }

        public virtual DynamicObject LoadReferenceData(DynamicObjectType dt, object pkValue)
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<object, DynamicObject> LoadReferenceDataBatch(DynamicObjectType dt, object[] pkValues)
        {
            throw new NotImplementedException();
        }

        public void SetValue(IDataEntityProperty prop, DynamicObject dataEntity, object value)
        {
            throw new NotImplementedException();
        }
    }
}
