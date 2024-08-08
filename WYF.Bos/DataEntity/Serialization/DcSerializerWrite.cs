using WYF.Bos.DataEntity.Metadata;
using JNPF.Form.DataEntity.Serialization;
using WYF;


namespace WYF.Bos.DataEntity.Serialization
{
    public abstract class DcSerializerWrite
    {

    

        internal DcBinder _binder;
        private readonly Dictionary<Type, Func<object, object, bool>> _equalsFuncCache;

        private readonly Dictionary<Type, Tuple<Func<ISimpleProperty, object, object, string>, bool>> _convertToStrFuncCache;

        protected readonly bool _serializeComplexProperty;

        protected readonly bool _isLocaleValueFull;

        public DcSerializerWrite(DcBinder binder, bool serializeComplexProperty, bool isLocaleValueFull)
        {
            this._binder = binder;
            this._convertToStrFuncCache = new Dictionary<Type, Tuple<Func<ISimpleProperty, object, object, string>, bool>>();
            this._serializeComplexProperty = serializeComplexProperty;
            this._isLocaleValueFull = isLocaleValueFull;
            this._equalsFuncCache = new Dictionary<Type, Func<object, object, bool>>();
        }


        protected Func<ISimpleProperty, object, object, string> GetConvertFunc(Type propertyType, out bool isCData)
        {
            Tuple<Func<ISimpleProperty, object, object, string>, bool> tuple;
            if (!this._convertToStrFuncCache.TryGetValue(propertyType, out tuple))
            {
                Func<ISimpleProperty, object, object, string> item = this.CreateConvertFunc(propertyType, out isCData);
                tuple = new Tuple<Func<ISimpleProperty, object, object, string>, bool>(item, isCData);
                this._convertToStrFuncCache.Add(propertyType, tuple);
            }
            isCData = tuple.Item2;
            return tuple.Item1;
        }


        protected Func<object, object, bool> GetEqualsFunc(Type propertyType)
        {
            Func<object, object, bool> func;
            if (!this._equalsFuncCache.TryGetValue(propertyType, out func))
            {
                func = this.CreateEqualsFunc(propertyType);
                this._equalsFuncCache.Add(propertyType, func);

            }
            return func;
        }

        private Func<object, object, bool>? CreateEqualsFunc(Type propertyType)
        {
            Func<object, object, bool> func;
            if (typeof(byte[]).IsAssignableFrom(propertyType)){
                func = (object currentValue, object baseValue) =>
                {
                    if (currentValue == null)
                    {
                        return baseValue == null;
                    }
                    byte[] array = (byte[])currentValue;
                    byte[] array2 = (byte[])baseValue;
                    if (array.Length != array2.Length)
                    {
                        return false;
                    }
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != array2[i])
                        {
                            return false;
                        }
                    }
                    return true;
                };
            }
            else
            {
                func = new Func<object, object, bool>(object.Equals);
            }

            func = this._binder.BindEqualsFunc(propertyType, func);
            if (func == null)
            {
                throw new ArgumentNullException("BindEqualsFunc");
            }
            return func;

        }

        private Func<ISimpleProperty, object, object, string> CreateConvertFunc(Type propertyType, out bool isCData)
        {
            Func<ISimpleProperty, object, object, string> func = new Func<ISimpleProperty, object, object, string>((ISimpleProperty sp, Object dataEntity, object value) =>
            {
                string result = value.ChangeType<string>();

                return result;
            });

            func = this._binder.BindToStringFunc(propertyType, func, out isCData);
            if (func == null)
            {
                throw new ArgumentNullException("BindToStringFunc");
            }
            return func;
        }
    }
}
