using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.DataEntity.Utils;

namespace WYF.DataEntity.Serialization
{
    public class DataEntitySerializerWriter
    {
        private DataEntitySerializerOption serOption;
        private Dictionary<string, Dictionary<object, object[]>> refComplexPropertys = new Dictionary<string, Dictionary<object, object[]>>();
        private IDataEntityBinder dataEntityBinder;
        public DataEntitySerializerWriter(): this(null)
        {
            
        }

        public DataEntitySerializerWriter(DataEntitySerializerOption option)
        {
            if (option == null)
            {
                this.serOption = new DataEntitySerializerOption();
            }
            else
            {
                this.serOption = option;
            }
            this.dataEntityBinder = this.serOption.DataEntityBinder;
        }


        public IDictionary<string, object> SerializerToMap(object dataEntity)
        {
            IDataEntityType dt;
            IDictionary<string, object> objMap = new Dictionary<string, object>();
            if (dataEntity is DynamicObject) {

                dt = ((DynamicObject)dataEntity).GetDataEntityType();
            }
            else
            {
                dt = OrmUtils.GetDataEntityType(dataEntity.GetType());
            }
            if (this.serOption.IsIncludeType)
                objMap["dt"] = WriteObjectType(dt);
            objMap["data"] = WriteObject(dataEntity, dt);
            if (this.serOption.IsIncludeComplexProperty)
                objMap["ref"] = this.refComplexPropertys;
            if (this.serOption.IsIncludeDataEntityState)
                objMap["state"] = true;
            return objMap;
        }

        private string GetExtendName(IDataEntityType dt)
        {
            if (dt is DynamicObjectType)
            {
               return ((DynamicObjectType)dt).ExtendName;
            }
            return dt.Name;
        }
        private Dictionary<string, object> WriteObjectType(IDataEntityType dt)
        {
            var dtMap = new Dictionary<string, object>
            {
                { "name", dt.Name },
                { "extname", GetExtendName(dt) }
            };

            var props = new List<Dictionary<string, object>>();
            dtMap["properties"] = props;

            foreach (var p in dt.Properties)
            {
                var prop = new Dictionary<string, object>
                {
                    { "name", p.Name }
                };

                if (!(p is ISimpleProperty))
                {
                    if (p is IComplexProperty complexProp)
                    {
                        prop["type"] = "cmp";
                        prop["dt"] = WriteObjectType(complexProp.ComplexType);
                    }
                    else if (p is ICollectionProperty collectionProp)
                    {
                        prop["type"] = "cp";
                        prop["dt"] = WriteObjectType(collectionProp.ItemType);
                    }
                }

                props.Add(prop);
            }

            return dtMap;
        }



        private object[] WriteObject(object dataEntity, IDataEntityType dt)
        {
            var values = new object[dt.Properties.Count() + (this.serOption.IsIncludeDataEntityState && dataEntity is DataEntityBase ? 1 : 0)];

            foreach (var p in dt.Properties)
            {
                if (!dataEntityBinder.IsSerializProperty(p, serOption))
                    continue;

                var value = p.GetValueFast(dataEntity);

                //if (p is ILocaleProperty localeProp)
                //{
                //    value = localeProp.DefaultItem;
                //}
                //else
                if (p is ISimpleProperty simpleProp)
                {
                    if (value is decimal)
                    {
                        value = value.ToString();
                    }
                    else if (value is DateTime)
                    {
                        value = ((DateTime)value).Ticks;
                    }
                }
                else if (p is IComplexProperty complexProp)
                {
                    var id = WriteComplexProperty(value, complexProp.ComplexType);
                    if (string.IsNullOrWhiteSpace(complexProp.RefIdPropName))
                    {
                        values[p.Ordinal] = id;
                    }
                    continue;
                }
                else if (p is ICollectionProperty collectionProp)
                {
                    values[p.Ordinal] = WriteCollectionProperty((List<object>)value, collectionProp.ItemType);
                }

                values[p.Ordinal] = value;
            }

            if (this.serOption.IsIncludeDataEntityState && dataEntity is DataEntityBase dataEntityBase)
            {
                var entityState = WriteDataEntityState(dataEntityBase.DataEntityState);
                if (entityState != null)
                {
                    values[dt.Properties.Count()] = entityState;
                }
            }

            return values;
        }
        public object[] WriteCollectionProperty(List<object> objs, IDataEntityType dt)
        {
            object[] collection = new object[objs.Count];
            int index = 0;

            foreach (var dataEntity in objs)
            {
                collection[index] = WriteObject(dataEntity, dt);
                index++;
            }

            return collection;
        }


        public Dictionary<string, object> WriteDataEntityState(DataEntityState state)
        {
            var ret = new Dictionary<string, object>();
            bool isFromDb = state.IsFromDatabase;
            ret["isfromdb"] = isFromDb;
            ret["bizchanged"] = state.GetBizChangeFlags();
            ret["dirtyprops"] = state.GetDirtyFlags();

            if (state.RemovedItems)
            {
                ret["rmitems"] = state.RemovedItems;
            }

            PkSnapshotSet pkSnapshots = state.PkSnapshotSet;
            if (pkSnapshots != null && pkSnapshots.Snapshots.Count > 0)
            {
                var mapPk = new Dictionary<string, object>(pkSnapshots.Snapshots.Count);
                foreach (var pk in pkSnapshots.Snapshots)
                {
                    if (pk.Opids == null)
                    {
                        mapPk[pk.TableName] = new object[] { pk.Oids };
                        continue;
                    }
                    mapPk[pk.TableName] = new object[] { pk.Oids, pk.Opids };
                }
                ret["pksnapshots"] = mapPk;
            }

            var entryInfos = state.EntryInfos;
            if (entryInfos != null)
            {
                ret["entryinfos"] = entryInfos;
            }

            return ret;
        }

        private object WriteComplexProperty(object dataEntity, IDataEntityType dt)
        {
            if (dataEntity == null)
                return null;

            string extendName = GetExtendName(dt);
            Dictionary<object, object[]> refObj;

            if (!refComplexPropertys.TryGetValue(extendName, out refObj))
            {
                refObj = new Dictionary<object, object[]>();
                refComplexPropertys[extendName] = refObj;
            }

            var primaryKey = dt.PrimaryKey;
            object id = primaryKey.GetValueFast(dataEntity);

            if (!refObj.ContainsKey(id))
            {
                object[] values = WriteObject(dataEntity, dt);
                refObj[id] = values;
            }

            return id;
        }


    }
}