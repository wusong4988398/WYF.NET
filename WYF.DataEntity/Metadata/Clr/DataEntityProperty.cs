
using JNPF.Form.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.Clr
{
    /// <summary>
    /// 实体属性基类
    /// </summary>
    public class DataEntityProperty : IDataEntityProperty, IMetadata
    {
        protected MethodInfo readMethod;

        protected String name;

        IDataEntityType parent;

        private MethodInfo writeMethod;
        private int ordinal;

        private bool isDbIgnore;

        private Type propertyType;


        public DataEntityProperty(PropertyInfo propertyInfo, int ordinal)
        {
            this.ordinal = ordinal;
            this.readMethod = propertyInfo.GetGetMethod();
            this.writeMethod = propertyInfo.GetSetMethod();
            this.propertyType = propertyInfo.PropertyType;
            this.name = propertyInfo.Name;
        }
        public IDataEntityType Parent { 
            get { return parent; }
            set { parent = value; }
        }
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType => this.propertyType;
        /// <summary>
        /// 属性是否只读
        /// </summary>
        public bool IsReadOnly => this.writeMethod == null;
        /// <summary>
        /// 属性位置
        /// </summary>
        public int Ordinal => this.ordinal;
        /// <summary>
        ///是否有缺省值
        /// </summary>
        public bool HasDefaultValue => false;
        /// <summary>
        /// 属性名
        /// </summary>
        public string Name => this.name;
        /// <summary>
        ///物理字段名
        /// </summary>
        public string Alias => null;
        /// <summary>
        /// 是否关联物理表 false:关联，true：不关联
        /// </summary>
        public bool IsDbIgnore { get { return this.isDbIgnore; } set { this.isDbIgnore = value; } }

        public bool IsDefinedDbIgnoreAttribute => false;

        public object Clone()
        {
         
            return base.MemberwiseClone();
        }

        /// <summary>
        /// 给定一个实体 读取此属性的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        /// <exception cref="BussinessException"></exception>
        public object GetValue(object dataEntity)
        {
            if (dataEntity==null) throw new Exception("CLR实体获取属性值失败，实体对象不能为空！");
            return this.readMethod.Invoke(dataEntity,null);

        }

        public object GetValueFast(object dataEntity)
        {
            return GetValue(dataEntity);
        }

        public bool IsEmpty(object dataEntity)
        {
            throw new Exception("不支持此方法！");
        }
        /// <summary>
        /// 为指定的属性设置值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="value"></param>
        /// <exception cref="BussinessException"></exception>
        public void SetValue(object dataEntity, object value)
        {
     

            if (this.writeMethod == null)
            {
               throw new Exception("属性只读");
            }
            if (dataEntity == null)
            {
                throw new Exception("CLR实体设置属性值失败，要设置的实体对象不能为空！");
            }

        
            this.writeMethod.Invoke(dataEntity, new Object[] { value });

      
        }

        public void SetValueFast(object dataEntity, object value)
        {
            SetValue(dataEntity, value);
        }
    }
}
