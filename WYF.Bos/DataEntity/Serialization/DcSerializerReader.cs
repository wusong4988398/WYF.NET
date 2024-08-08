using WYF.Bos.DataEntity;
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.Form.DataEntity.Serialization
{
    public abstract class DcSerializerReader
    {
        private static readonly string STRING = "??????";

        protected DcBinder _binder;
        protected Stack<ISupportInitialize> _supportInitializeObjects;
        protected bool privateOnlyLocaleValue;
        bool privateResetLoacaleValueBy2052;
        IList _lastList;
        ICollectionProperty _lastColProperty;
        Object _lastColEntity;
        protected bool _isLocaleValueFull;


        public DcSerializerReader(DcBinder binder, bool isLocaleValueFull)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }
            else
            {
                this._binder = binder;
                this._supportInitializeObjects = new Stack<ISupportInitialize>();
                this._isLocaleValueFull = isLocaleValueFull;
            }
        }



        protected IList SafeGetList(ICollectionProperty property, String itemElementName, Object entity)
        {
            if (object.ReferenceEquals(this._lastColProperty, property) && object.ReferenceEquals(this._lastColEntity, entity))
            {
                return this._lastList;
            }
            object obj2 = property.GetValue(entity);
            if (obj2 == null)
            {
                if (property.IsReadOnly)
                {
                    SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                    {
                        CanIgnore = true
                    };
                    this.ThrowXmlException("014009000001748", $"集合属性{property.Name}是只读且未初始化值，请初始化的值或提供Set功能。", data, null);
                }
                try
                {
                    obj2 = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(entity, obj2);
                }
                catch (Exception exception)
                {
                    SerializationException.SerializationExceptionData data2 = new SerializationException.SerializationExceptionData
                    {
                        CanIgnore = true
                    };
                    this.ThrowXmlException("014009000001749", $"自动创建集合属性{property.Name}的值失败，{exception.Message}。", data2, exception);
                }
            }
            IList list = null;
            if (obj2 is DataEntityPropertyCollection)
            {
                list = obj2 as DynamicPropertyCollection;
            }
            else
            {
                 list = (obj2 is IList) ? (IList)obj2 : null;
            }
            
            //IList list = obj2 as IList;
            if (list == null) {
                SerializationException.SerializationExceptionData data3 = new SerializationException.SerializationExceptionData
                {
                    CanIgnore = true
                };
                this.ThrowXmlException("014009000001750", $"集合属性{property.Name}必须支持IList接口", data3, null);
            }
            this._lastColProperty = property;
            this._lastColEntity = entity;
            this._lastList = list;
            return list;

        }


        public object ConvertFromString(ISimpleProperty property, object dataEntity, string str)
        {
            if (property.PropertyType==typeof(string))
            {
                return str;
            }else if (property.PropertyType == typeof(bool))
            {
               return bool.Parse(str);
            }
            //System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType);
            //object propValue = typeConverter.ConvertFromString(str);
            object propValue = Convert.ChangeType(str, property.PropertyType);
            return propValue;
        }


        protected void ThrowXmlException(string code, string message, SerializationException.SerializationExceptionData data, Exception innerException = null)
        {
            data.OnReading = true;
            this._binder.ThrowException(new SerializationException(code, message, data, innerException));
        }

    }
}
