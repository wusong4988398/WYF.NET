using WYF.DataEntity.Metadata;

using WYF.DataEntity.Serialization;

using System.Collections;
using WYF;
using WYF.Bos.DataEntity;

namespace WYF.DataEntity.Serialization
{
    public class DcJsonSerializerWriteImplement : DcSerializerWrite
    {



        public DcJsonSerializerWriteImplement(DcBinder binder, bool serializeComplexProperty, bool isLocaleValueFull) 
            : base(binder, serializeComplexProperty, isLocaleValueFull)
        {

        }


        public string Serialize(IDataEntityType dt, object currentEntity, object baseEntity)
        {
            JSONObject jsonobject = this.WriteObject(dt, currentEntity, baseEntity, null);
            return jsonobject.ToString();
        }

        private JSONObject WriteObject(IDataEntityType currentEntityType, object currentEntity, object baseEntity, DcAction action)
        {
            if (currentEntity == null)
            {
                throw new NotSupportedException();
            }
            JSONObject jsonobject;
            currentEntityType = this.WriteObjectElement(currentEntityType, currentEntity, baseEntity, action, out jsonobject);
            if (this.ShouldCompare(currentEntityType, baseEntity))
            {
                if (currentEntityType.Flag == DataEntityTypeFlag.Primitive)
                {
                    return jsonobject;
                }
                if (currentEntityType.Flag != DataEntityTypeFlag.Primitive)
                {
                    foreach (var p in currentEntityType.Properties)
                    {
                        if (!this._binder.IsOnlyDbProperty || !p.IsDbIgnore)
                        {
                            ISimpleProperty simpleProperty = p as ISimpleProperty;

                            if (simpleProperty!=null)
                            {
                                this.WriteSimpleProperties(jsonobject, simpleProperty, currentEntity, baseEntity);
                                continue;
                            }

                            IComplexProperty complexProperty = p as IComplexProperty;
                            if (complexProperty != null && this._serializeComplexProperty)
                            {
                                this.WriteComplexProperties(jsonobject, complexProperty, currentEntity, baseEntity);
                                continue;
                            }

                            ICollectionProperty collectionProperty = p as ICollectionProperty;
                            if (collectionProperty != null)
                            {

                                this.WriteCollectionProperties(jsonobject, collectionProperty, currentEntityType, currentEntity, baseEntity);

                            }


                        }
                    }
                }


            }else if (currentEntityType.Flag == DataEntityTypeFlag.Primitive)
            {
                WritePrimitiveObject(currentEntityType, currentEntity);
            }
            else
            {
                foreach (IDataEntityProperty dataEntityProperty2 in currentEntityType.Properties) {
                    if (!this._binder.IsOnlyDbProperty || !dataEntityProperty2.IsDbIgnore)
                    {
                        ISimpleProperty simpleProperty = dataEntityProperty2 as ISimpleProperty;
                        if (simpleProperty != null)
                        {
                            this.WriteSimpleProperties_S(jsonobject, simpleProperty, currentEntity);
                            continue;
                        }
                        IComplexProperty complexProperty = dataEntityProperty2 as IComplexProperty;
                        if (complexProperty != null && this._serializeComplexProperty)
                        {
                            this.WriteComplexProperties_S(jsonobject, complexProperty, currentEntity);
                            continue;
                        }

                        ICollectionProperty collectionProperty = dataEntityProperty2 as ICollectionProperty;
                        if (collectionProperty != null)
                        {
                            this.WriteCollectionProperties_S(jsonobject, collectionProperty, currentEntity);
                            continue;
                        }

                    }



                }
            }

            this._binder.AfterWriteJsonObject(currentEntity, baseEntity, jsonobject);

            return jsonobject;
        }

        private IDataEntityType WriteObjectElement(IDataEntityType dt, object entity, object baseEntity, DcAction action, out JSONObject serObj)
        {
            JSONObject jsonobject = new JSONObject();
            IDataEntityType dataEntityType = this._binder.GetDataEntityType(entity);
            if (dataEntityType != null && !dataEntityType.Equals(dt))
            {
                dt = dataEntityType;
            }
            jsonobject["Name"] = this._binder.BindToName(dt);
            if (dt == null)
            {
                throw new Exception($"实体{entity.GetType().Name}必须实现IDataEntityBase才能进行序列化操作");
            }
            serObj = new JSONObject();
            IDictionary<string, string> dataEntityAttributes = this._binder.GetDataEntityAttributes(entity);
            if (dataEntityAttributes != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in dataEntityAttributes)
                {
                    jsonobject.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            ISimpleProperty primaryKey = dt.PrimaryKey;
            string pkValue = null;
            if (primaryKey != null)
            {
                object pkTemp = primaryKey.GetValue(entity);
                if (pkTemp != null)
                {
                    pkValue = this.ConvertToString(primaryKey, entity, pkTemp);
                }
            }
            if (baseEntity == null && action == null)
            {
                action = DcAction.ListAction_Add;
            }
            if (action != null && !DcAction.ListAction_Add.Equals(action))
            {
                jsonobject.Add("action", action.ActionName);
                jsonobject.Add("oid", pkValue);
            }
            if (jsonobject.Count > 0)
            {
                if (jsonobject.Count==1&& jsonobject.ContainsKey("Name"))
                {
                    serObj["_Type_"] = jsonobject.Get("Name");
                }
                else
                {
                    serObj["_Type_"] = jsonobject;
                }
               

               
            }
            return dt;

        }


        private void WriteSimpleProperties_S(JSONObject serObj, ISimpleProperty property, object entity)
        {
            if (property.ShouldSerializeValue(entity))
            {
                object value = property.GetValue(entity);
                if (value == null)
                {
                    JSONObject jsonobject = new JSONObject();
                    jsonobject["action"] = DcAction.PropertyAction_SetNull.ActionName;
                    serObj[property.Name] = jsonobject;
                    return;
                }

                if (property.PropertyType == typeof(string))
                {
                    bool flag;
                    string text = this.ConvertToString(property, entity, value, out flag);
                    text = ((text == null) ? string.Empty : text);
                    serObj[property.Name] = text;
                    return;
                }
                else
                {
                    serObj[property.Name] = value;
                }

            }
        }


        private void WriteComplexProperties_S(JSONObject serObj, IComplexProperty property, object entity)
        {
            object value = property.GetValue(entity);
            if (value != null)
            {
                JSONObject value2 = this.WriteObject(property.ComplexType, value, null, null);
                if (DcJsonSerializerWriteImplement.IsNeedWrite(value2))
                {
                    serObj[property.Name] = value2;
                }
            }
        }


        private void WriteCollectionProperties_S(JSONObject serObj, ICollectionProperty property, object entity)
        {
            this.WriteCollectionProperty(serObj, property, property.GetValue(entity));
        }

        private void WriteComplexProperties(JSONObject serObj, IComplexProperty property, object currentEntity, object baseEntity)
        {
            Object currentValue = property.GetValue(currentEntity);
            Object baseValue = property.GetValue(baseEntity);
            if (currentValue == null)
            {
                if (baseValue != null)
                {
                    serObj[property.Name] = DcAction.PropertyAction_SetNull.ActionName;
                }

            }
            else
            {
                
              JSONObject value = this.WriteObject(property.ComplexType, currentValue, baseValue, null);
                if (DcJsonSerializerWriteImplement.IsNeedWrite(value))
                {
                    serObj[property.Name] = value;
                }

            }
        }
        private void WriteSimpleProperties(JSONObject serObj, ISimpleProperty property, object currentEntity, object baseEntity)
        {
            Object currentValue = property.GetValue(currentEntity);
            Object baseValue = property.GetValue(baseEntity);
            Func<object, object, bool> equalsFunc = this.GetEqualsFunc(property.PropertyType);
            if (!equalsFunc(currentValue, baseValue))
            {
                if (currentValue == null)
                {
                    JSONObject jsonobject = new JSONObject();
                    jsonobject["action"] = DcAction.PropertyAction_SetNull.ActionName;
                    serObj[property.Name] = jsonobject;
                    return;
                }
                if (!property.ShouldSerializeValue(currentEntity))
                {
                    JSONObject jsonobject2 = new JSONObject();
                    jsonobject2["action"] = DcAction.PropertyAction_Reset.ActionName;
                    serObj[property.Name] = jsonobject2;
                    return;
                }
                if (property.PropertyType == typeof(string))
                {
                    bool flag;
                    string valueToString = this.ConvertToString(property, currentEntity, currentValue, out flag);
                    serObj[property.Name] = valueToString;
                    return;
                }
                serObj[property.Name] = currentValue;

            }

        }


        private void WriteCollectionProperties(JSONObject serObj,  ICollectionProperty property,  IDataEntityType currentEntityType, object currentEntity, object baseEntity)
        {

            object currentValue = property.GetValue(currentEntity);
            object baseValue = property.GetValue(baseEntity);
            if (currentValue == null)
            {
                if (baseValue != null) serObj[property.Name] = DcAction.PropertyAction_SetNull.ActionName;
            }
            else if(baseEntity==null)
            {
                WriteCollectionProperty(serObj, property, currentValue);
            }
            else
            {
                Func<string> getMessage = () => $"在序列化对象{currentEntityType.Name}的集合属性{property.Name}时";
                IEnumerable<object> currentList = currentValue as IEnumerable<object>;

                if (currentList == null)
                {
                    throw new System.Runtime.Serialization.SerializationException($"发现其(当前列表)属性的返回类型{property.PropertyType.Name}不支持IEnumerable接口。");
                }
                IEnumerable<object> baseList = baseValue as IEnumerable<object>;

                if (baseList == null)
                {
                    throw new System.Runtime.Serialization.SerializationException($"发现其(基列表)属性的返回类型{property.PropertyType.Name}不支持IEnumerable接口。");
                }
                JSONArray lstvalue = new JSONArray();

                ListSync.Sync<object, object>(currentList, baseList,
                    (s, t) => this.ListItemEquatable(currentList, baseList, s, t, getMessage),
                    (s, t) =>
                    {
                        this.WriteEditObject(lstvalue, property.ItemType, s, t);
                    },
                    s => s,
                    (tList, t) =>
                    {
                        this.WriteAddObject(lstvalue, property.ItemType, t);
                    },
                    (tList, t, index) =>
                    {
                        this.WriteRemoveObject(lstvalue, t);
                    },
                    true);


                if (lstvalue.Count > 0)
                {
                    serObj[property.Name] = lstvalue;
                }
            }

        }

        private void WriteCollectionProperty(JSONObject serObj, ICollectionProperty property, object value)
        {
            IList list = value as IList;
            if (list != null && list.Count > 0)
            {
                JSONArray jsonarray = new JSONArray();
                foreach (object currentEntity in list)
                {
                    jsonarray.Add(this.WriteObject(property.ItemType, currentEntity, null, DcAction.ListAction_Add));
                }
                serObj[property.Name] = jsonarray;
            }
        }

            private string ConvertToString(ISimpleProperty property, object dataEntity, object value)
        {
            bool flag;
            Func<ISimpleProperty, object, object, string> convertFunc = this.GetConvertFunc(property.PropertyType, out flag);
            return convertFunc(property, dataEntity, value);
        }
        private string ConvertToString(ISimpleProperty property, object dataEntity, object value, out bool isCData)
        {
            Func<ISimpleProperty, object, object, string> convertFunc = this.GetConvertFunc(property.PropertyType, out isCData);
            return convertFunc(property, dataEntity, value);
        }

        private String WritePrimitiveObject(IDataEntityType dt, Object value)
        {
            string str = value.ToNullString();
            return str;
        }
        public static bool IsNeedWrite(object value)
        {
            if (value is JSONObject)
            {
                JSONObject jsonobject = (JSONObject)value;
                if (jsonobject.Count == 1)
                {
                    object obj;
                    if (jsonobject.TryGetValue("_Type_", out obj))
                    {
                        return false;
                    }
                }
                else if (jsonobject.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void WriteAddObject(JSONArray jarray, IDataEntityType currentEntityType, object currentEntity)
        {
            JSONObject item = this.WriteObject(currentEntityType, currentEntity, null, DcAction.ListAction_Add);
            jarray.Add(item);
        }

        private void WriteEditObject(JSONArray jarray, IDataEntityType currentEntityType, object currentEntity, object baseEntity)
        {
            JSONObject jsonobject = this.WriteObject(currentEntityType, currentEntity, baseEntity, DcAction.ListAction_Edit);
            if (DcJsonSerializerWriteImplement.IsNeedWrite(jsonobject))
            {
                jarray.Add(jsonobject);
            }
        }


        private void WriteRemoveObject(JSONArray jarray, object t)
        {
            IDataEntityType dataEntityType = this._binder.GetDataEntityType(t);
            ISimpleProperty primaryKey = dataEntityType.PrimaryKey;
            JSONObject jsonobject = new JSONObject();
            jsonobject["_Type_"] = new JSONObject
            {
                {
                    "action",
                    DcAction.ListAction_Remove.ActionName
                },
                {
                    "oid",
                    this.ConvertToString(primaryKey, t, primaryKey.GetValue(t))
                }
            };
            jarray.Add(jsonobject);
        }

        private bool ListItemEquatable(IEnumerable sList, IEnumerable tList, object s, object t, Func<string> getMessage)
        {
            ISimpleProperty primaryKey = this._binder.GetDataEntityType(s).PrimaryKey;
            if (primaryKey == null)
            {
                throw new System.Runtime.Serialization.SerializationException(getMessage() + $"发现其明细类型{this._binder.GetDataEntityType(s).Name}没有定义主键.");
            }

            object value = primaryKey.GetValue(s);
            if (value == null)
            {
                int num = -1;
                IList list = sList as IList;
                if (list != null)
                {
                    num = list.IndexOf(s);
                }
                throw new System.Runtime.Serialization.SerializationException($"发现(当前列表)其第{num}的主键为空，序列化前必须填充主键。");
            }
            ISimpleProperty primaryKey2 = this._binder.GetDataEntityType(t).PrimaryKey;
            if (primaryKey2 == null)
            {
                throw new System.Runtime.Serialization.SerializationException($"发现其明细类型{this._binder.GetDataEntityType(t).Name}没有定义主键。");

            }
            object value2 = primaryKey2.GetValue(t);
            if (value2 == null)
            {
                int num2 = -1;
                IList list2 = tList as IList;
                if (list2 != null)
                {
                    num2 = list2.IndexOf(t);
                }
            
                throw new System.Runtime.Serialization.SerializationException($"发现(基列表)其第{num2}的主键为空，序列化前必须填充主键。");
            }
            return object.Equals(value, value2);
        }

        private bool ShouldCompare(IDataEntityType currentEntityType, object baseEntity)
        {
            if (baseEntity != null)
            {
                IDataEntityType dataEntityType = this._binder.GetDataEntityType(baseEntity);
                if (currentEntityType.Equals(dataEntityType))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
