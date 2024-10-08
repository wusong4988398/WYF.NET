

using WYF.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WYF.DataEntity.Metadata.Dynamicobject
{
    [Serializable]
    public class DynamicSimpleProperty : DynamicProperty, ISimpleProperty
    {
    

 

     

        public bool IsLocalizable { get; set; }


        public virtual int DbType { get; set; }



        [SimpleProperty(Name = "EnableNull")]
        public bool IsEnableNull { get; set; }



        [SimpleProperty(Name = "PrimaryKey")]
        public  bool IsPrimaryKey {  get; set; }
       


        [SimpleProperty]
        public string TableGroup { get; set; }

        [SimpleProperty]
        public bool IsEncrypt { get; set; }

        public DynamicSimpleProperty() { }
        public DynamicSimpleProperty(string name, Type propertyType, object defaultValue) : base(name, propertyType, defaultValue, false)
        {
            if (defaultValue == null)
            {
                if (propertyType.IsPrimitive)
                {
                    if (propertyType == typeof(bool))
                    {
                        this._defaultValue = false;
                    }
                    else if (propertyType == typeof(char))
                    {
                        this._defaultValue = ' ';
                    }
                    else
                    {
                        
                        this._defaultValue = Convert.ChangeType(0, propertyType);
                    }
                }
            }
            else if (!propertyType.IsInstanceOfType(defaultValue))
            {
                try
                {
                    
                    this._defaultValue =  Convert.ChangeType(defaultValue, propertyType);
                }
                catch (Exception e)
                {

                    throw new Exception($"对实体类型{this.Name}注册简单属性{name}失败,缺省值{defaultValue}({defaultValue.GetType()})与属性类型{propertyType}不一致。");
         
                }
            }
        }
        private static DynamicObject ConvertToDynamicObject(Object dataEntity)
        {
            DynamicObject obj = (dataEntity is DynamicObject) ? (DynamicObject)dataEntity : null;
            if (obj == null)
            {
                if (dataEntity == null) throw new Exception($"转换对象为动态实体失败，要转换的对象不能为空！");
            }
            return obj;
        }
        public  void ResetDTValue(DynamicObject dataEntity)
        {
            DynamicProperty find = FindTrueProperty(dataEntity);
            find.ResetValuePrivate(dataEntity);
        }
        public void ResetValue(object dataEntity)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            ResetDTValue(obj);
        }

        public void SetDirty(object dataEntity, bool newValue)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            obj.DataEntityState.setDirty(this, newValue);
        }

        public bool ShouldSerializeValue(object dataEntity)
        {
            DynamicObject obj = ConvertToDynamicObject(dataEntity);
            IDataEntityProperty trueProperty = FindTrueProperty(obj);
            return (obj.DataStorage.getLocalValue(trueProperty) != null);
        }
    }
}
