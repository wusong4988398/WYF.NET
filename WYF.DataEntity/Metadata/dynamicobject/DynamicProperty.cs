
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Clr;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.Dynamicobject
{
    /// <summary>
    /// 动态属性的基类,对实体字段值的操作是通过它来完成的.
    /// </summary>
    [Serializable]
    public class DynamicProperty : DynamicMetadata, IDataEntityProperty, ICloneable
    {
        protected String _name;

        private String _alias = "";

        private string _displayName;

        protected Type _propertyType;


        IDataEntityType _parent;

        protected Object _defaultValue;

        protected bool _hasDefaultValue;

        protected bool _isReadonly;

        protected int _cachedHashcode;

        protected int _ordinal = -1;
        internal object[] _attributes;

        [SimpleProperty]
        public string DisplayName { get; set; }

        public IDictionary<String, Object> CustomProperties { get; set; } = new ConcurrentDictionary<String, Object>();

        [NonSerialized]
        private PropertyChangedEventArgs _propertyChangedEventArgsCache;
        /// <summary>
        /// 父实体类型
        /// </summary>
        public virtual IDataEntityType Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        /// <summary>
        ///此属性的返回类型
        /// </summary>
        public virtual Type PropertyType => this._propertyType;
        /// <summary>
        /// 返回此属性是否是只读属性
        /// </summary>
        public bool IsReadOnly => this._isReadonly;
        /// <summary>
        /// 返回此属性在引用的实体类型中所在的位置
        /// </summary>
        public int Ordinal
        {
            get => this._ordinal;
            set { this._ordinal = value; }
        }
        /// <summary>
        /// 此属性是否有缺省值
        /// </summary>
        public bool HasDefaultValue => this._hasDefaultValue;
        /// <summary>
        /// 属性名称
        /// </summary>
        [SimpleProperty]
        public override string Name { 

            get { return this._name; } 
            set { this._name = value; } 
        }
        /// <summary>
        /// 属性对应的数据库字段
        /// </summary>
        [SimpleProperty]
 
        public override string Alias
        {
            get { return this._alias; }
            set { this._alias = value; }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue
        {
            get
            {
                return this._defaultValue;
            }
            set 
            { 
                this._defaultValue = value;
                this._hasDefaultValue = InnerHasDefaultValue();
            }
        }

        public void AfterClone()
        {
        }

        public DynamicProperty() { }


        public DynamicProperty(String name, Type propertyType, Object defaultValue, bool isReadonly)
        {
            if (!IsValidIdentifier(name))throw new Exception($"{name}不符合规范");

            if (propertyType == null)throw new Exception($"propertyType不能为空");

            this._name = name;
            this._propertyType = propertyType;
            this._defaultValue = defaultValue;
            this._isReadonly = isReadonly;
            this._hasDefaultValue = InnerHasDefaultValue();
        }




        #region Method
        public override object[] GetCustomAttributes(bool inherit)
        {
            if (this._attributes == null)
            {
                return DynamicMetadata.EmptyAttributes;
            }
            return this._attributes;
        }

        private static DynamicObject ConvertToDynamicObject(object dataEntity)
        {
            if (dataEntity == null) throw new Exception($"转换对象为动态实体失败，要转换的对象不能为空！");
          
            if (!(dataEntity is DynamicObject)) throw new Exception($"转换对象{dataEntity.ToString()}为动态实体失败，该对象必须是DynamicObject类型！");
            return (DynamicObject)dataEntity;
        }
        private bool InnerHasDefaultValue()
        {
            if (this._defaultValue == null)
                return false;
            if (this.PropertyType == typeof(int))
            {
                if (this._defaultValue.Equals(0))
                    return false;
            }
            else if (this.PropertyType == typeof(long))
            {
                if (this._defaultValue.Equals(0L))
                    return false;
            }
            else if (this.PropertyType == typeof(decimal) && this._defaultValue.Equals(new decimal(0)))
            {
                return false;
            }
            return (this._defaultValue != null);
        }

        public object Clone()
        {
            return base.Clone();
        }
        /// <summary>
        /// 给定一个实体，读取此属性描述符在此实体的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public object GetValue(object dataEntity)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            return GetDTValue<object>(obj);
        }


        public void ResetDTValue(DynamicObject dataEntity)
        {
            DynamicProperty find = FindTrueProperty(dataEntity);
            find.ResetValuePrivate(dataEntity);
        }

        public void ResetValuePrivate(DynamicObject dataEntity)
        {
            if (this._isReadonly)
                throw new Exception("DynamicProperty.ResetValuePrivate  ReadOnlyException");
            Object newValue = this._defaultValue;
            SetValuePrivate(dataEntity, newValue);
        }
        /// <summary>
        /// 获取给定属性的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public Object GetValueFast(Object dataEntity)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            return GetDTValueFast(obj);
        }
        /// <summary>
        /// 实体属性是否为空值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public bool IsEmpty(object dataEntity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 为给定实体的属性填充指定的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="value"></param>
        public void SetValue(object dataEntity, object value)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            SetDTValue(obj, value);
        }
        public  void SetDTValue(DynamicObject dataEntity, Object newValue)
        {
            DynamicProperty find = FindTrueProperty(dataEntity);
            find.SetValuePrivate(dataEntity, newValue);
        }

        protected void SetValuePrivate(DynamicObject dataEntity, object newValue)
        {
            if (this._isReadonly) throw new Exception($"DynamicProperty.SetValuePrivate  ReadOnlyException");
            object oldValue = null;
            if (!dataEntity.Initializing)
                oldValue = dataEntity.DataStorage.getLocalValue(this);
            if (dataEntity.IsInitializing || oldValue == null || !object.Equals(newValue, oldValue))
            {
                if (newValue != null)
                {
                    Type newValueType = newValue.GetType();
                    Type pType = this.PropertyType;
                    if (IsTypeNotEquals(newValueType, pType))
                    {
                        if (string.IsNullOrEmpty(newValue as string))
                        {
                            if (pType == typeof(DateTime))
                            {
                                newValue = null;
                            }
                            else if (pType == typeof(decimal))
                            {
                                newValue = 0;
                            }
                        }
                        else if (pType == typeof(DateTime) && newValueType == typeof(int))
                        {
                            long longTime = long.Parse(((int)newValue).ToString());
                            newValue = Convert.ChangeType(longTime, pType);
                        }
                        else
                        {
                            newValue = Convert.ChangeType(newValue, pType);
                        }
                    }
                }
                if (newValue == null || object.Equals(newValue, this.DefaultValue))
                {
                    dataEntity.DataStorage.setLocalValue(this, null);
                }
                else
                {
                    dataEntity.DataStorage.setLocalValue(this, newValue);
                }
                OnPropertyChanged(dataEntity, newValue, oldValue);
            }
        }

        protected void OnPropertyChanged(DynamicObject dataEntity, Object newValue, Object oldValue)
        {
            bool isChange = false;
            if (dataEntity.IsResetDirtyFlag)
            {
                isChange = dataEntity.IsResetDirtyFlag;
            }
            else
            {
                Object v1 = (oldValue == null) ? this.DefaultValue : oldValue;
                Object v2 = (newValue == null) ? this.DefaultValue : newValue;
                isChange = (!dataEntity.IsInitializing && !object.Equals(v1, v2));
            }
            if (isChange)
            {

                //DataEntityPropertyChangedEventArgs e = new DataEntityPropertyChangedEventArgs(this, dataEntity, oldValue, newValue);
           
                dataEntity.OnPropertyChanged(this.PropertyChangedEventArgs);
            }
        }

        internal PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get
            {
                if (this._propertyChangedEventArgsCache == null)
                {
                    this._propertyChangedEventArgsCache = new DataEntityPropertyChangedEventArgs(this);
                }
                return this._propertyChangedEventArgsCache;
            }
        }
        private bool IsTypeNotEquals(Type newValueType, Type pType)
        {
            if (newValueType == typeof(long) && pType == typeof(long))
                return false;
            if (newValueType == typeof(bool) && pType == typeof(bool))
                return false;
            if (pType == null)
                return false;
            return (newValueType != pType && !pType.IsAssignableFrom(newValueType));
        }
        /// <summary>
        /// 返回此属性被引用的类型
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DynamicProperty FindTrueProperty(DynamicObject dataEntity)
        {
            if (dataEntity == null) throw new Exception($"寻找实体上{this.Name}对应的属性描述符失败，实体不能为空！");
           
            DynamicObjectType dt = dataEntity.DynamicObjectType;
            if (this.Parent == dt)
                return this;
            DataEntityPropertyCollection properties = dt.Properties;
            DynamicProperty find = (DynamicProperty)properties[this.Name];
            if (find != null)
                return find;
            StringBuilder sb = new StringBuilder();
            foreach (IDataEntityProperty p in dataEntity.DynamicObjectType.Properties)
                sb.Append(p.Name).Append(" ");

            throw new Exception($"寻找实体上{this.Name}对应的属性描述符失败，实体不存在此属性！  [EntityType：{dataEntity.DynamicObjectType.Name} Propeyties:{sb}]");

        }

        /// <summary>
        /// 给定一个实体，为给定实体的属性填充指定的值，在确定dataEntity实体类型和此属性对应实体类型一致时采用此方法
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="value"></param>
        public void SetValueFast(object dataEntity, object value)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            SetDTValueFast(obj, value);
        }

        public void SetDTValueFast(DynamicObject dataEntity, Object newValue)
        {
            SetValuePrivate(dataEntity, newValue);
        }
        /// <summary>
        /// 从实体中检索当前属性的值并直接返回指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataEntity">要检索的实体对象</param>
        /// <returns>此实体中此属性的值</returns>
        public T GetDTValue<T>(DynamicObject dataEntity)
        {
            DynamicProperty find = FindTrueProperty(dataEntity);
            return (T)find.GetDTValueFast(dataEntity);
        }
        /// <summary>
        /// 从实体中快速检索当前属性的值,在确定dataEntity实体类型和此属性对应实体类型一致时采用此方法
        /// </summary>
        /// <param name="dataEntity">要检索的实体对象</param>
        /// <returns>此实体中此属性的值</returns>
        public virtual object GetDTValueFast(DynamicObject dataEntity)
        {
            Object localValue = dataEntity.DataStorage.getLocalValue(this);
            if (localValue == null)
                return this.DefaultValue;
            return localValue;
        }


        private bool IsValidIdentifier(String name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return true;
        }
        #endregion


    }
}
