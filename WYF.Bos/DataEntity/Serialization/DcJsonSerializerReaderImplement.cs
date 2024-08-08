
using WYF.Bos.DataEntity;
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace JNPF.Form.DataEntity.Serialization
{
    public sealed class DcJsonSerializerReaderImplement : DcSerializerReader
    {
        private static readonly string STRING = "??????";
        private Dictionary<Type, Action<ISimpleProperty, object, object>> _setValueActionsCache = new Dictionary<Type, Action<ISimpleProperty, object, object>>();
    
        public DcJsonSerializerReaderImplement(DcBinder binder, bool isLocaleValueFull) : base(binder, isLocaleValueFull)
        {

        }
        /// <summary>
        /// 为指定属性填充指定的值
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private Action<ISimpleProperty, object, object> CreateSetValueAction(Type propertyType)
        {
            Action<ISimpleProperty, object, object> setValueAction;

            if (typeof(string).IsAssignableFrom(propertyType))
            {
                setValueAction = (property, token, entity) =>
                {
                    string str = token?.ToString();
                    property.SetValue(entity, str);
                };
            }
            else if (typeof(bool).IsAssignableFrom(propertyType))
            {
                setValueAction = (property, token, entity) =>
                {
                    property.SetValue(entity, (bool)token);
                };
            }
            else if (typeof(int).IsAssignableFrom(propertyType))
            {
                setValueAction = (property, token, entity) =>
                {
                    property.SetValue(entity, Convert.ToInt32(token));
                };
            }
            else if (typeof(long).IsAssignableFrom(propertyType))
            {
                setValueAction = (property, token, entity) =>
                {
                    property.SetValue(entity, Convert.ToInt64(token));
                };
            }
            else if (typeof(byte[]).IsAssignableFrom(propertyType))
            {
                setValueAction = (property, token, entity) =>
                {
                    property.SetValue(entity, (byte[])token);
                };
            }
            else
            {
                setValueAction = (property, token, entity) =>
                {
                    string str = token?.ToString() ?? string.Empty;
                    object obj2 = ConvertFromString(property, entity, str);
                    property.SetValue(entity, obj2);
                };
            }

            setValueAction = this._binder.BindJSONReadAction(propertyType, setValueAction);

            if (setValueAction == null)
            {
                throw new ArgumentNullException("BindReadDataAction");
            }

            return setValueAction;
        }


        public object ReadElement(JSONObject token, IDataEntityType dt, object entity)
        {
            string name = GetElementName(token, "Name", null);
            if (entity == null)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    dt = this.BindToType(token, name, dt);
                }
                if (dt.Flag != DataEntityTypeFlag.Primitive)
                {
                    entity = this._binder.CreateInstance(dt);
                }
            }
            else
            {
                dt = this.GetDataEntityType(entity);
            }
            this.Push(entity);
            if (dt.Flag != DataEntityTypeFlag.Primitive)
            {
                foreach (KeyValuePair<string, object> pair in token)
                {
                    IDataEntityProperty property;
                    if (dt.Properties.TryGetValue(pair.Key, out property))
                    {
                        if (this.ReadSimpleProperty(pair.Value, property as ISimpleProperty, entity))
                        {
                            Console.WriteLine("ReadSimpleProperty");
                        }
                        else if (this.ReadComplexProperty(pair.Value, property as IComplexProperty, entity))
                        {
                            Console.WriteLine("ReadComplexProperty");
                        }
                        else if (this.ReadCollectionProperty(pair.Value, property as ICollectionProperty, entity))
                        {
                            Console.WriteLine("ReadCollectionProperty");
                        }
                        else
                        {
                            SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                            {
                                CanIgnore = true
                            };
                            this.ThrowXmlException("??????", $"JSON节点中出现的属性{property.Name}，必须是简单属性、复杂或集合属性的一种。", data, null);
                        }
                    }
                    else if (pair.Key!="_Type_" && pair.Key != "Type")
                    {
                        this._binder.ReadCustomJsonProperty(pair, entity);
                    }
                    
                }
            }
                return entity;
        }


        private void Push(object entity)
        {
            ISupportInitialize item = entity as ISupportInitialize;
            if (item != null)
            {
                item.BeginInit();
                this._supportInitializeObjects.Push(item);
            }
        }
        private IDataEntityType GetDataEntityType(Object entity)
        {
            return this._binder.GetDataEntityType(entity);
        }
        private IDataEntityType BindToType(JSONObject token, string name, IDataEntityType canUseType)
        {
            object obj2;
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            if (token.TryGetValue(name, out obj2))
            {
                foreach (KeyValuePair<string, object> pair in (JSONObject)obj2)
                {
                    attributes.Add(pair.Key, pair.Value.ToString());
                }
            }
            IDataEntityType? dt = this._binder.BindToType(name, attributes);
            if (dt == null && canUseType != null)
            {
                string a = this._binder.BindToName(canUseType);
                if (string.Equals(a, name, StringComparison.Ordinal)|| canUseType is DynamicObjectType)
                {
                    dt = canUseType;
                }
            }

            if (dt == null)
            {
                SerializationException.SerializationExceptionData tempVar = new SerializationException.SerializationExceptionData();
                tempVar.CanIgnore=false;
                this.ThrowXmlException("??????", $"未能找到{name}对应的数据类型，请检查是否Json字符串和类型注册", tempVar, null);
            }

            return dt;
        }

        private static string GetElementName(JSONObject obj, string attName, string defaultValue)
        {
            
            string str = defaultValue;
            object prop = obj["_Type_"];
            return prop is string ? (string)prop : GetAttributeValue(obj, "Name", null);


            
        }

        private static string GetAttributeValue(JSONObject obj, string attName, string defaultValue)
        {
            string value = defaultValue;
            Object prop = obj["_Type_"];
            if (prop is JSONObject) {
                JSONObject attrs = (JSONObject)prop;
                if (attrs != null)
                {
                    value = (string)attrs[attName];
                }
            }
            return value;
        }

        private bool ReadSimpleProperty(object token, ISimpleProperty property, object entity)
        {
            string action;
            if (property == null)
            {
                return false;
            }
            if (token is JSONObject)
            {
                action = GetValue<string>((JSONObject)token, "action", "setvalue");
            }
            else
            {
                action = "setvalue";
            }
            this.DoSimplePropertyAction(token, action, property, entity);

            return true;
        }


        private bool ReadComplexProperty(object token, IComplexProperty property, object entity)
        {
            if (property==null)
            {
                return false;
            }else if (!(token is JSONObject))
            {
                return false;
            }
            else if (!(token is JSONObject) &&string.IsNullOrEmpty(token.ToNullString()))
            {
                return true;
            }
            else
            {
                string action = "edit";
                JSONObject jObj = (JSONObject)token;
                action = GetAttributeValue(jObj, "action", action);
                this.DoComplexPropertyAction(jObj, action, property, entity);
                return true;
            }
        }


        private bool ReadCollectionProperty(object token, ICollectionProperty property, object entity)
        {
            if (property == null)
            {
                return false;
            }
            foreach (JSONObject obj2 in (JSONArray)token)
            {
                string action = GetAttributeValue(obj2, "action", "add");
                this.DoCollectionPropertyAction(obj2, action, property, entity);
            }

            return true;

        }

        private void DoCollectionPropertyAction(JSONObject item, string action, ICollectionProperty property, object entity)
        {
            object findItem;
            IList list;
            switch (action)
            {
                case "add":
                     findItem = this.ReadElement(item, property.ItemType, null);
                     list = this.SafeGetList(property, "",entity);
               
                    
                    if (list != null)
                    {
                        if (list is DataEntityPropertyCollection)
                        {
                            ((DynamicPropertyCollection)list).Add((IDataEntityProperty)findItem);
                        }
                        else
                        {
                            list.Add(findItem);
                        }

                   
                        


                    }
                    break;
                case "edit":
                    object obj3;
                    int num = this.FindItemByOid(item, property, entity, out obj3);

                    break;

                default:
                    break;
            }
        }

        private void DoComplexPropertyAction(JSONObject token, string action, IComplexProperty property, Object entity)
        {
            if (action== "edit")
            {
                object currentValue = property.GetValue(entity);
                object newValue = this.ReadElement(token, property.ComplexType, currentValue);
                if (!property.IsReadOnly && !object.ReferenceEquals(currentValue, newValue))
                {
                    property.SetValue(entity, newValue);
                }

            }
            else if (action == "setnull")
            {
                property.SetValue(entity, null);
            }
            else
            {
                SerializationException.SerializationExceptionData tempVar = new SerializationException.SerializationExceptionData();
                this.ThrowXmlException("??????", $"不能识别的属性操作符{action}", tempVar, null);
            }
        }

        private void DoSimplePropertyAction(object token, string action, ISimpleProperty property, object entity)
        {
            switch (action)
            {
                case "setvalue":
                    try
                    {
                        this.GetSetValueAction(property.PropertyType)(property, token, entity);

                    }
                    catch (Exception exception)
                    {

                        SerializationException.SerializationExceptionData data = new SerializationException.SerializationExceptionData
                        {
                            CanIgnore = true
                        };
                        this.ThrowXmlException(STRING, $"在赋值{property.Name}:{property.PropertyType.Name}的值'{token.ToString()}'时失败，{exception.Message}", data, exception);
                    }
                    break;
                case "reset":
                    property.ResetValue(entity);
                    break;
                case "setnull":
                    try
                    {
                        property.SetValue(entity, null);
                    }
                    catch (Exception exception)
                    {


                        SerializationException.SerializationExceptionData data2 = new SerializationException.SerializationExceptionData
                        {
                            CanIgnore = true
                        };
                        this.ThrowXmlException(STRING, $"在赋值{property.Name}:{property.PropertyType.Name}的值 Null 时失败，{exception.Message}" , data2, exception);
                    }
                    break;

                default:

                    SerializationException.SerializationExceptionData data3 = new SerializationException.SerializationExceptionData
                    {
                        CanIgnore = true
                    };
                    this.ThrowXmlException(STRING, $"不能识别的属性操作符{action}" ,data3, null);

                    break;
            }
        }
        private Action<ISimpleProperty, object, object> GetSetValueAction(Type propertyType)
        {
            Action<ISimpleProperty, object, object> action;
            if (!this._setValueActionsCache.TryGetValue(propertyType, out action))
            {
                action = this.CreateSetValueAction(propertyType);
                this._setValueActionsCache.Add(propertyType, action);
            }
            return action;
        }
        private static T GetValue<T>(JSONObject obj, string propName, T defaultValue)
        {

            object obj2;
            if (obj.TryGetValue(propName, out obj2))
            {
                return (T)obj2;
            }
            return defaultValue;
        }

        private int FindItemByOid(JSONObject srcItem, ICollectionProperty property, object entity, out object item)
        {
            item = null;
            return 0;
        }

        public void EndInitialize()
        {
       
            while (this._supportInitializeObjects.Count > 0)
            {
                this._supportInitializeObjects.Pop().EndInit();
            }

        }
    }
}
