
using WYF.Bos.DataEntity;
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using JNPF.Form.DataEntity.Utils;
using System.Xml;

namespace JNPF.Form.DataEntity.Serialization
{
    public abstract class DcBinder
    {
        public static readonly String ELEMENT = "Element";
        private bool ignoreCase = false;
        private bool onlyDbProperty = true;
        private String lcid;
        private bool serializeDefaultValue;
        private static Dictionary<string, Type> _primitiveTypes = new Dictionary<string, Type>();
        static DcBinder()
        {
            _primitiveTypes.Add("boolean", typeof(bool));
            _primitiveTypes.Add("bool", typeof(bool));
            _primitiveTypes.Add("byte", typeof(byte));
            _primitiveTypes.Add("sByte", typeof(sbyte));
            _primitiveTypes.Add("int16", typeof(short));
            _primitiveTypes.Add("iint16", typeof(ushort));
            _primitiveTypes.Add("int32", typeof(int));
            _primitiveTypes.Add("int", typeof(int));
            _primitiveTypes.Add("uint32", typeof(uint));
            _primitiveTypes.Add("int64", typeof(long));
            _primitiveTypes.Add("uint64", typeof(ulong));
            _primitiveTypes.Add("intptr", typeof(IntPtr));
            _primitiveTypes.Add("uintptr", typeof(UIntPtr));
            _primitiveTypes.Add("char", typeof(char));
            _primitiveTypes.Add("double", typeof(double));
            _primitiveTypes.Add("single", typeof(float));
            _primitiveTypes.Add("string", typeof(string));
            _primitiveTypes.Add("decimal", typeof(decimal));
            _primitiveTypes.Add("guid", typeof(Guid));
            _primitiveTypes.Add("datetime", typeof(DateTime));
            _primitiveTypes.Add("timespan", typeof(TimeSpan));
            _primitiveTypes.Add("datetimeoffset", typeof(DateTimeOffset));

        }
        public DcBinder()
        {
            
        }

        public bool IsIgnoreCase
        {
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }
        public bool IsSerializeDefaultValue 
        { 
          get { return serializeDefaultValue; } 
          set {  serializeDefaultValue = value; }
        }

        public bool IsOnlyDbProperty
        {
            get { return onlyDbProperty; }
            set
            {
                onlyDbProperty = value;
            }
        }

        public string Lcid
        {
            get {
                if (string.IsNullOrEmpty(lcid))
                {
                    lcid = "zh_CN";
                }
                return lcid; 
            }
            set
            {
                lcid = value;
            }
        }

        public virtual string BindToName(IDataEntityType dt)
        {
            return dt.Name;
        }
        public static Type GetPrimitiveType(String elementName)
        {
            return (elementName == null) ? null : _primitiveTypes[elementName.ToLower()];
        }


        public Dictionary<String, String> GetDataEntityAttributes(Object dataEntity)
        {
            return null;
        }


        public void ThrowException(SerializationException serializationException)
        {
            if (!serializationException.ExceptionData.CanIgnore)
            {
                throw serializationException;
            }
        }

        public Object CreateInstance(IDataEntityType dt)
        {
            return dt.CreateInstance();
        }
        public IDataEntityType? BindToType(string elementName, Dictionary<string, string> attributes)
        {
            if (elementName != null)
            {
                IDataEntityType tempVar = this.TryBindToType(elementName, attributes);
                if (tempVar != null)
                {
                    return tempVar;
                }
                Type type = _primitiveTypes.GetValueOrDefault(elementName, null);
          
                if (type != null)
                {
                    return OrmUtils.GetDataEntityType(type);
                }
            }

            return null;
        }


        public IDataEntityType GetDataEntityType(object dataEntity)
        {
            if (dataEntity == null) return null;

            IDataEntityBase entityBase = (dataEntity is IDataEntityBase) ? (IDataEntityBase)dataEntity : null;
            if (entityBase != null)
                return entityBase.DataEntityType;
            return OrmUtils.GetDataEntityType(dataEntity.GetType());
        }

        protected virtual Action<ISimpleProperty, XmlReader, object> BindReadAction(Type dataType, Action<ISimpleProperty, XmlReader, object> defaultAction )
        {
            return defaultAction;
        }

        public virtual Action<ISimpleProperty, object, object> BindJSONReadAction(Type dataType, Action<ISimpleProperty, object, object> defaultAction)
        {
            return defaultAction;
        }

        public Func<object, object, bool> BindEqualsFunc(Type dataType, Func<object, object, bool> defaultFunc)
        {
            return defaultFunc;
        }

        public Func<ISimpleProperty, object, object, string> BindToStringFunc(Type dataType, Func<ISimpleProperty, object, object, string> defaultFunc, out bool isCData)
        {
            isCData= false;
            return defaultFunc;
        }

        public void AfterWriteJsonObject(object currentEntity, object baseEntity, Dictionary<String, object> serObj) { }

        public bool WriteSimpleProperty(XmlReader serObj, ISimpleProperty property, object entity)
        {
            return false;
        }


        public bool ReadSimpleProperty(ISimpleProperty property, XmlReader propElement, object entity)
        {
            return false;
        }
        public abstract IDataEntityType TryBindToType(String elementName, Dictionary<String, String> attributes);

        public abstract void ReadCustomJsonProperty(KeyValuePair<string, object> pair, object entity);
        
    }
}
