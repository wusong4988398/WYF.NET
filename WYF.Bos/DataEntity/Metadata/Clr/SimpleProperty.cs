


using AspectCore.Extensions.Reflection;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.Clr
{
    public sealed class SimpleProperty : DataEntityProperty, ISimpleProperty
    {


        private bool shouldSerializeQueried;

        private bool defaultValueQueried;

        private bool primaryKey;

        private String alias;
        private int dbType;

        private bool encrypt;

        private bool enableNull;

        private String tableGroup;

        private MethodInfo shouldSerializeMethod;

        private Object _defaultValue;


        public SimpleProperty(PropertyInfo propertyInfo, int ordinal) : base(propertyInfo, ordinal)
        {
            this._defaultValue = _noValue;
            //SimplePropertyAttribute spAtt =this.readMethod.GetCustomAttribute<SimplePropertyAttribute>();

            SimplePropertyAttribute spAtt=propertyInfo.GetCustomAttribute<SimplePropertyAttribute>();


            this.name = !string.IsNullOrEmpty(spAtt.Name) ? spAtt.Name : propertyInfo.Name;
            this.primaryKey = spAtt.IsPrimaryKey;
            this.alias = spAtt.Alias;
            this.dbType = spAtt.DbType;
            this.IsDbIgnore = spAtt.IsDbIgnore;
            this.encrypt = spAtt.IsEncrypt;
            this.tableGroup = spAtt.TableGroup;
            this.enableNull = spAtt.IsEnableNull;


        }
        private static Object _noValue = new Object();


        public bool IsLocalizable => false;
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey 
        { 
            get { return this.primaryKey; } 
            set { this.primaryKey = value; } 
        }

        /// <summary>
        /// 是否允许为NULL
        /// </summary>
        public bool EnableNull
        {
            get { return this.enableNull; }
            set { this.enableNull = value; }
        }

        

        public new string Alias
        {
            get { return this.alias; }
            set { this.alias = value; }
        }

        public string TableGroup
        {
            get { return this.tableGroup; }
            set { this.tableGroup = value; }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public int DbType
        {
            get { return this.dbType; }
            set { this.dbType = value; }
        }
        /// <summary>
        /// 是否加密
        /// </summary>
        public bool IsEncrypt
        {
            get { return this.encrypt; }
            set { this.encrypt = value; }
        }

        static ISet<Type> numberTypes = new HashSet<Type>() { };
        
    /// <summary>
    /// 给定一个实体 重置此属性的值
    /// </summary>
    /// <param name="dataEntity"></param>
    public void ResetValue(object dataEntity)
        {
            if (!this.IsReadOnly)
            {
                Object defValue = GetDefaultValue(dataEntity);
                if (defValue != _noValue)
                SetValue(dataEntity, defValue);
            }
        }


        private object GetDefaultValue(object dataEntity)
        {
            if (!this.defaultValueQueried)
            {
                object defaultvalue = null;
                bool tempVar = PropertyInfoUtils.TryGetDefaultValue(this.readMethod, ref defaultvalue);
                this._defaultValue = defaultvalue;
                if (!tempVar) { this._defaultValue = _noValue; }
                this.defaultValueQueried = true;
            }
            return this._defaultValue;
        }

        public void SetDirty(object dataEntity, bool newValue)
        {
            
        }

        public new bool IsEmpty(object dataEntity)
        {
            Object v = GetValue(dataEntity);
            return IsEmptyValue(v, this.PropertyType);
        }
        public bool IsEmptyValue(object value, Type propertyType)
        {
            if (value == null) return true;
           

            if (propertyType.IsValueType && value.Equals(Activator.CreateInstance(propertyType)))
            {
                 return true;
            }
            else if (propertyType == typeof(string) && string.IsNullOrEmpty((string)value))
            {
                return true;
            }
            else if (propertyType.IsClass && value == null)
            {
                return true;
            }
            else if(typeof(DateTime).IsAssignableFrom(propertyType))
            {
                return value == null|| default(DateTime)==(DateTime)value;
            }
            else
            {
                throw new Exception("not support!!!");
            }
        }


        private MethodInfo GetShouldSerializeMethod()
        {
            if (!this.shouldSerializeQueried)
            {
                this.shouldSerializeMethod = PropertyInfoUtils.GetShouldSerializeMethod(this.readMethod.GetBindingProperty());
                this.shouldSerializeQueried = true;
            }
            return this.shouldSerializeMethod;
        }

        public bool ShouldSerializeValue(Object dataEntity)
        {
            if (GetShouldSerializeMethod() != null)
                try
                {
                    if (!object.ReferenceEquals(this.GetDefaultValue(dataEntity), _noValue))
                    {
                        return ((bool)this.shouldSerializeMethod.Invoke(dataEntity, null));
                    }
                }
                catch (Exception exception) { }
            Object defaultValue = GetDefaultValue(dataEntity);
            if (!object.ReferenceEquals(defaultValue, _noValue))
            {
                return !object.Equals(defaultValue, base.GetValue(dataEntity));
            }
               
            return true;
        }

        
    }
}
