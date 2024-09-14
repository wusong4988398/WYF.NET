


using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    /// <summary>
    /// 动态实体对象,对应ORM的持久化对象 单据采用此对象存储数据(单据数据包)
    /// </summary>
    [Serializable]
    public class DynamicObject : DataEntityBase
    {
        private DynamicObjectType dt;

        private IDataStorage dataStorage;

  
        private bool isQueryObj;

        public DynamicObject() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dt">当前动态实体的类型，不可为空。</param>
        /// <param name="id">主键值对象</param>
        /// <exception cref="BussinessException"></exception>
        public DynamicObject(DynamicObjectType dt, Object id)
        {
            if (dt == null) throw new Exception("构造动态实体失败，构造参数：实体类型dt不能为空!");
            if (dt.Flag== DataEntityTypeFlag.Abstract) throw new Exception($"构造动态实体失败，{dt.Name}为抽象类型，不允许被实例化！");
            if (dt.Flag == DataEntityTypeFlag.Interface) throw new Exception($"构造动态实体失败，{dt.Name}为接口类型，不允许被实例化！");
            this.dt = dt;
            this.dataStorage = CreateDataStorage();
            if (id != null)
                dt.PrimaryKey.SetValue(this, id);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dt">当前动态实体的类型，不可为空</param>
        public DynamicObject(DynamicObjectType dt):this(dt, null)
        {
            
        }
        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="dt">当前动态实体的类型，不可为空</param>
        /// <param name="isQueryObj">是否查询对象</param>
        public DynamicObject(DynamicObjectType dt, bool isQueryObj): this(dt, null)
        {
            
            this.isQueryObj = isQueryObj;
        }
        public DynamicObjectType DynamicObjectType => this.dt;
        protected internal IDataStorage DataStorage
        {
            get
            {
                return this.dataStorage;
            }
            internal set
            {
                this.dataStorage = value;
            }
        }
        protected virtual IDataStorage CreateDataStorage()
        {

            if (this.dt.Properties.Count > 200)
            {
                return new DictionaryDataStorage(this.dt);
            }
            return new ArrayStorage(this.dt);
        }


        public override IDataEntityType GetDataEntityType()
        {
            return (IDataEntityType)this.dt;
        }

        public object this[int index]
        {
            get
            {
                IDataEntityProperty dp = this.dt.Properties[index];
                if (dp != null)
                {
                    Object value = Convert.ChangeType(dp.GetValueFast(this), dp.PropertyType);
                    if (value == null && dp.IsEnableNull)
                    {
                        return null;
                    }
                    return value;

                }
                return null;
            }
            set
            {
                this.dt.Properties[index].SetValue(this, value);
            }
        }

        public object this[string propertyName]
        {
            get
            {
                
                IDataEntityProperty dp = this.dt.Properties[propertyName];
                if (dp!= null)
                {
                    Object value=  Convert.ChangeType(dp.GetValueFast(this), dp.PropertyType);
                    if (value == null && dp.IsEnableNull)
                    {
                        return null;
                    }
                    return value;
                        
                }
                if (propertyName.IndexOf('.') == -1 && propertyName.LastIndexOf(']') == -1)
                {
                    dp = (IDataEntityProperty)this.dt.Properties[propertyName];
                    if (dp == null) throw new Exception($"实体类型{this.dt.Name}中不存在名为{propertyName}的属性");
                    Object value = Convert.ChangeType(dp.GetValueFast(this), dp.PropertyType);
                    if (value == null && dp.IsEnableNull)
                    {
                        return null;
                    }
                    return value;
                }
                return GetPropertyPathValue(propertyName);
            }
            set
            {
                IDataEntityProperty dp = this.dt.Properties[propertyName];
                if (dp != null)
                {
                    dp.SetValueFast(this, value);
                    return;
                }
                
            }
        }

        public object this[IDataEntityProperty property]
        {
            get
            {
                if (property == null)
                {
                    throw new ArgumentNullException("property");
                }
                return property.GetValueFast(this);
            }
            set
            {
                if (property == null)
                {
                    throw new ArgumentNullException("property");
                }
                property.SetValue(this, value);
            }
        }

        private Object GetPropertyPathValue(String propertyPath)
        {
            String[] segs = propertyPath.Split("\\.");
            Object obj = this;
            foreach (var seg in segs)
            {
                obj = GetPropertyValue(obj, seg);
            }
            return obj;
        
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
                return null;
            int index = -1;
            int s1 = propertyName.IndexOf("[");
            if (s1 != -1)
            {
                int s2 = propertyName.IndexOf("]");
                if (s1 != -1 && s2 > s1)
                {
                    string s = propertyName.Substring(s1 + 1, s2 - s1 - 1);
                    if (Regex.IsMatch(s, "\\d+"))
                        index = int.Parse(s);
                }
                if (index == -1)
                    throw new ArgumentException("Porperty incorrect: " + propertyName);
            }
            if (index == -1)
            {
                if (obj is DynamicObject)
                    return ((DynamicObject)obj)["propertyName"];
                if (obj is IDictionary)
                    return ((IDictionary)obj)[propertyName];
                string m1 = propertyName.ToLower();
                string m2 = "get" + m1;
                string m3 = "is" + m1;
                try
                {
                    foreach (MethodInfo m in obj.GetType().GetMethods())
                    {
                        if (m.GetParameters().Length == 0)
                        {
                            string name = m.Name.ToLower();
                            if (name.Equals(m1) || name.Equals(m2) || name.Equals(m3))
                            {
                                m.Invoke(obj, new object[0]);
                                return m.Invoke(obj, new object[0]);
                            }
                        }
                    }
                    foreach (FieldInfo field in obj.GetType().GetFields())
                    {
                        if (field.Name.ToLower().Equals(m1))
                        {
                            field.SetValue(obj, field.GetValue(obj));
                            return field.GetValue(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException(obj.GetType() + " get property failed: " + propertyName, e);
                }
            }
            else
            {
                obj = GetPropertyValue(obj, propertyName.Substring(0, s1));
                if (obj != null)
                {
                    if (obj is IList)
                        return ((IList)obj)[index];
                    throw new ArgumentException(obj.GetType() + " property is not a list: " + propertyName);
                }
            }
            return null;
        }

        [Serializable]
        internal struct ArrayStorage : IDataStorage
        {

            private object[] _values;
            private ArrayStorage(object[] values)
            {
                this._values = values;
            }
            public ArrayStorage(DynamicObjectType dt)
            {
                this._values = new object[dt.Properties.Count];
            }
            public object getLocalValue(IDataEntityProperty property)
            {
                int index = property.Ordinal;
                if (index >= this._values.Length)
                    EnsureCapacity(property);
                return this._values[index];
            }

            public IDataStorage memberClone()
            {
                object[] array = new object[this._values.Length];
                this._values.CopyTo(array, 0);
                return new DynamicObject.ArrayStorage(array);
            }

            public void setLocalValue(IDataEntityProperty property, object value)
            {
                int index = property.Ordinal;
                if (index >= this._values.Length)
                    EnsureCapacity(property);
                this._values[index] = value;
            }
            private void EnsureCapacity(IDataEntityProperty property)
            {
                int newSize = property.Ordinal;
                Array.Resize<object>(ref this._values, newSize);
            }
        }

        internal struct DictionaryDataStorage : IDataStorage
        {
            private ConcurrentDictionary<IDataEntityProperty, object> _values;

            private DictionaryDataStorage(ConcurrentDictionary<IDataEntityProperty, object> values)
            {
                this._values = values;
            }
            public DictionaryDataStorage(DynamicObjectType dt)
            {
                this._values = new ConcurrentDictionary<IDataEntityProperty, object>();
            }
            public object getLocalValue(IDataEntityProperty property)
            {
                object obj2;
                this._values.TryGetValue(property, out obj2);
                return obj2;
            }

            public IDataStorage memberClone()
            {

                ConcurrentDictionary<IDataEntityProperty, object> values = new ConcurrentDictionary<IDataEntityProperty, object>();
                foreach (KeyValuePair<IDataEntityProperty, object> pair in this._values)
                {
                    values[pair.Key] = pair.Value;
                }
                return new DynamicObject.DictionaryDataStorage(values);
            }

            public void setLocalValue(IDataEntityProperty property, object value)
            {
                this._values[property] = value;
            }
        }
    }
}
