
using WYF.DataEntity.Entity;
using System;


namespace WYF.DataEntity.Metadata.Dynamicobject
{
    /// <summary>
    /// 动态实体架构的基础元数据对象
    /// </summary>
    [Serializable]
    public abstract class DynamicMetadata : IMetadata
    {
        private bool isDbIgnore;
        public static object[] EmptyAttributes = new object[0];
        /// <summary>
        /// 是否关联物理表 false:关联，true：不关联
        /// </summary>
        [SimpleProperty(Name = "DbIgnore")]
        public virtual bool IsDbIgnore 
        { 
            get 
            { 
                return isDbIgnore; 
            } 
            set 
            { 
                isDbIgnore = value; 
            } 
        }

        /// <summary>
        /// 元数据对象的名称
        /// </summary>
        public abstract string Name { get;  set; }
        /// <summary>
        /// 物理字段名
        /// </summary>
        public abstract string Alias { get;  set; }



        public abstract object[] GetCustomAttributes(bool inherit);
        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            object[] customAttributes = this.GetCustomAttributes(inherit);
            if ((customAttributes == null) || (customAttributes.Length <= 0))
            {
                return EmptyAttributes;
            }
            List<object> list = new List<object>(customAttributes.Length);
            foreach (object obj2 in customAttributes)
            {
                if (attributeType.IsInstanceOfType(obj2))
                {
                    list.Add(obj2);
                }
            }
            return list.ToArray();
        }


        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj.GetType() == GetType()) {
                return isEquals(obj);
            }

            // TODO: write your implementation of Equals() here

            return base.Equals(obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return createHashCode();
            //return base.GetHashCode();
        }
        protected bool isEquals(Object obj)
        {
            return string.Equals(Name, ((DynamicMetadata)obj).Name);
         
        }
        public int createHashCode()
        {
            return (string.IsNullOrEmpty(Name)) ? 0 : Name.GetHashCode();
        }

        public object Clone() 
        {
            
           return base.MemberwiseClone();
        }

}
}
