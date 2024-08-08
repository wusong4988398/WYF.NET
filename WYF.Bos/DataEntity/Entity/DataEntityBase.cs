
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;
using JNPF.Form.DataEntity.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Entity
{
    /// <summary>
    /// 数据包基类，DynamicObject的父类
    /// </summary>
    [Serializable]
    public abstract class DataEntityBase : INotifyPropertyChanged, ISupportInitializeNotification,ISupportInitialize, IDataEntityBase, IObjectWithParent
    {
        [NonSerialized]
        private EventHandlerList _events;
        [NonSerialized]
        private bool _initializing;
        internal PkSnapshot[] _pkSnapshots;
        internal string _dirtyFlags;
        internal bool _fromDatabase;
        private BoolDataEntityState _state;
        [NonSerialized]
        private object _parent;
        private readonly bool _isResetDirtyFlag;
 
        private static readonly object InitializedEventKey = new object();
        private static readonly object PropertyChangedEventKey = new object();
        private static readonly object PropertyChangingEventKey = new object();
        protected EventHandlerList Events
        {
            get
            {
                if (this._events == null)
                {
                    this._events = new EventHandlerList();
                }
                return this._events;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.Events.AddHandler(PropertyChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PropertyChangedEventKey, value);
            }
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                this.Events.AddHandler(PropertyChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PropertyChangingEventKey, value);
            }
        }

        event EventHandler ISupportInitializeNotification.Initialized
        {
            add
            {
                this.Events.AddHandler(InitializedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(InitializedEventKey, value);
            }
        }

        /// <summary>
        /// 触发属性改变事件
        /// </summary>
        /// <param name="e">属性改变的事件参数</param>
        protected internal virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!this._initializing)
            {
                this.DataEntityState.SetPropertyChanged(e);
                if (this._events != null)
                {
                    
                    PropertyChangedEventHandler handler = (PropertyChangedEventHandler)this._events[PropertyChangedEventKey];
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
            }
        }

        protected virtual void OnInitialized()
        {
            if (this._events != null)
            {
                EventHandler handler = (EventHandler)this._events[InitializedEventKey];
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
        public void BeginInit()
        {
            this._initializing = true;
        }

        public void EndInit()
        {
            if (this._initializing)
            {
                this._initializing = false;
                this.OnInitialized();
            }
        }

        public bool IsInitializing
        {
            get
            {
                return this._initializing;
            }
        }

        public  bool IsResetDirtyFlag
        {
            get
            {
                return this._isResetDirtyFlag;
            }
        }


        /// <summary>
        /// 是否正在初始化过程中。
        /// </summary>
        internal bool Initializing
        {
            get
            {
                return this._initializing;
            }
        }

       

        /// <summary>
        /// 返回此实体对应的父对象
        /// </summary>
        public object Parent
        {
            get
            {
                return this._parent;
            }

            set { this._parent = value; }
        }


        

        /// <summary>
        /// 返回当前数据行的状态管理
        /// </summary>
        public DataEntityState DataEntityState
        {
            get
            {
                if (this._state == null)
                {
                    lock (this)
                    {
                        if (this._state == null)
                        {
                            BitArray array;
                            DataEntityPropertyCollection properties = this.GetDataEntityType().Properties;
                            if (string.IsNullOrEmpty(this._dirtyFlags))
                            {
                                array = new BitArray(properties.Count);
                            }
                            else
                            {
                                array = new BitArray(SerializationUtils.StringToIntArray(this._dirtyFlags))
                                {
                                    Length = properties.Count
                                };
                            }
                            this._state = new BoolDataEntityState(properties, array, this._pkSnapshots, this._fromDatabase);
                            this._pkSnapshots = null;
                            this._dirtyFlags = null;
                        }
                    }
                }
                return this._state;
            }
        }

  

        /// <summary>
        /// 获取主键值
        /// </summary>
        public object PkValue { 
            get
            {
                if (GetDataEntityType().PrimaryKey == null)
                    return null;
                ISimpleProperty pkProp = GetDataEntityType().PrimaryKey;
                return pkProp.GetValueFast(this);
            }
        }
        /// <summary>
        /// 返回当前实体的数据类型
        /// </summary>
        public IDataEntityType DataEntityType
        {
            get
            {
                return this.GetDataEntityType();
            }
        }

        bool ISupportInitializeNotification.IsInitialized
        {
            get
            {
                return !this._initializing;
            }
        }


        public abstract IDataEntityType GetDataEntityType();
      

    }
}
