using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using WYF.DataEntity.Metadata.Dynamicobject;


namespace WYF.DataEntity.Serialization
{
    public class DataEntitySerializerReader
    {
        private Dictionary<string, Dictionary<object, List<object>>> refComplexPropertyData = new Dictionary<string, Dictionary<object, List<object>>>();
        private Dictionary<string, Dictionary<object, object>> refComplexPropertys = new Dictionary<string, Dictionary<object, object>>();
        private DataEntityDeserializerOption option;
        private bool includeComplexProperty = false;
        private bool includeDataEntityState = false;
        public DataEntitySerializerReader(DataEntityDeserializerOption option)
        {
            if (option == null)
            {
                this.option = new DataEntityDeserializerOption();
            }
            else
            {
                this.option = option;
            }
        }

        public object ReadObject(IDataEntityType dt, IDictionary<string, object> mapObj)
        {
            List<object> values = (List<object>)mapObj.Get("data");
            object refDataObj = mapObj.Get("ref");
            if (refDataObj != null)
            {
                this.includeComplexProperty = true;
                this.refComplexPropertyData = (Dictionary<string, Dictionary<object, List<object>>>)refDataObj;
                this.refComplexPropertys = new Dictionary<string, Dictionary<object, object>>();
            }
            object state = mapObj.Get("state");
            
            if (state != null && state.ToBool())
            {
                this.includeDataEntityState = true;
            }
            object o = ReadObject(dt, values, GetPropertyMaps(dt));
            return o;
        }


        public object ReadObject(IDataEntityType currentDataEntityType, List<object> values, List<IPropertyMap> propMaps)
        {
            bool hasState = (this.includeDataEntityState || this.option.IsIncludeDataEntityState);
            if (!hasState)
                hasState = (values.Count == currentDataEntityType.Properties.Count + 1);

            object obj = currentDataEntityType.CreateInstance();
            if (obj is ISupportInitialize)
                ((ISupportInitialize)obj).BeginInit();

            foreach (var p in propMaps)
            {
                p.MapValue(obj, values);
            }
            if (hasState && obj is DataEntityBase)
            {
                var state = (IDictionary<string, object>)values[values.Count - 1];
                if (state != null)
                {
                    DataEntityBase dataEntityBase = (DataEntityBase)obj;
                    SetState(dataEntityBase.DataEntityState, state);
                }
            }

            if (obj is ISupportInitialize)
                ((ISupportInitialize)obj).EndInit();

            return obj;
        }

        private void SetState(DataEntityState state, IDictionary<string, object> doc)
        {
            if (doc == null)
                return;

            // 获取并设置 isfromdb 的值
            bool bValue = bool.Parse(doc["isfromdb"].ToString());
            
            state.SetFromDatabase(bValue);

            // 获取并设置 rmitems 的值
            object value = doc["rmitems"];
            if (value != null)
            {
                state.RemovedItems=value.ToBool();
            }

            // 获取并设置 pksnapshots 的值
            var pksnapshots = doc["pksnapshots"] as IDictionary<string, object>;
            if (pksnapshots != null)
            {
                state.PkSnapshotSet = FromPkSnapshotSet(pksnapshots);
            }

            // 获取并设置 bizchanged 的值
            var bizChangedList = doc["bizchanged"] as List<object>;
            if (bizChangedList != null)
            {
                int[] bizChangeds = new int[bizChangedList.Count];
                for (int i = 0; i < bizChangedList.Count; i++)
                {
                    var v = bizChangedList[i];
                    bizChangeds[i] = v is int ? (int)v : (int)v;
                }
                state.SetBizChangeFlags(bizChangeds);
            }

            // 获取并设置 dirtyprops 的值
            var dirtyList = doc["dirtyprops"] as List<object>;
            if (dirtyList != null)
            {
                int[] dirtyprops = new int[dirtyList.Count];
                for (int i = 0; i < dirtyList.Count; i++)
                {
                    var v = dirtyList[i];
                    dirtyprops[i] = v.ToInt();
                }
          
                state.SetDirtyFlags(dirtyprops);
            }
            // 获取并设置 entryinfos 的值
            var mapEntryInfos = doc["entryinfos"] as IDictionary<string, IDictionary<string, object>>;
            if (mapEntryInfos != null)
            {
                var entryInfos = new Dictionary<string, EntryInfo>(mapEntryInfos.Count);
                foreach (var mapEntry in mapEntryInfos)
                {
                    entryInfos.Add(mapEntry.Key, FromEntryInfo(mapEntry.Value));
                }
                state.EntryInfos = entryInfos;
            }
        }


        private EntryInfo FromEntryInfo(IDictionary<string, object> doc)
        {
            EntryInfo info = new EntryInfo();

            // 获取并设置 rowCount 的值
            if (doc.TryGetValue("rowCount", out var rowCount) && rowCount is int)
            {
                info.RowCount = (int)rowCount;
            }

            // 获取并设置 startRowIndex 的值
            if (doc.TryGetValue("startRowIndex", out var startRowIndex) && startRowIndex is int)
            {
                info.StartRowIndex = (int)startRowIndex;
            }

            // 获取并设置 pageSize 的值
            if (doc.TryGetValue("pageSize", out var pageSize) && pageSize is int)
            {
                info.PageSize = (int)pageSize;
            }

            return info;
        }


        private PkSnapshotSet FromPkSnapshotSet(IDictionary<string, object> doc)
        {
            if (doc == null || doc.Count == 0)
                return null;

            PkSnapshotSet set = new PkSnapshotSet();
            set.Snapshots = new List<PkSnapshot>(doc.Count);

            foreach (var table in doc)
            {
                PkSnapshot snap = new PkSnapshot();
                snap.TableName = table.Key;

                var pkarray = table.Value as List<object>;
                if (pkarray != null && pkarray.Count > 0)
                {
                    var oids = pkarray[0] as List<object>;
                    if (oids != null)
                    {
                        snap.Oids = new object[oids.Count];
                        for (int i = 0; i < oids.Count; i++)
                        {
                            var v = oids[i];
                            snap.Oids[i] = v is int ? (long)(int)v : v;
                        }
                    }

                    if (pkarray.Count > 1)
                    {
                        var opids = pkarray[1] as List<object>;
                        if (opids != null)
                        {
                            snap.Opids = new object[opids.Count];
                            for (int i = 0; i < opids.Count; i++)
                            {
                                var v = opids[i];
                                snap.Opids[i] = v is int ? (long)(int)v : v;
                            }
                        }
                    }
                }

                set.Snapshots.Add(snap);
            }

            return set;
        }

        private List<IPropertyMap> GetPropertyMaps(IDataEntityType dt)
        {
            List<IPropertyMap> propMaps = new List<IPropertyMap>();
            foreach (var p in dt.Properties)
            {
                //if (p is ILocaleProperty)
                //{

                //}
                if (p is ISimpleProperty)
                {
                    SimplePropertyMap prop = new SimplePropertyMap(p.Ordinal, p);
                    propMaps.Add(prop);
                    continue;
                }
                if (p is IComplexProperty&& this.includeComplexProperty)
                {
                    IComplexProperty cmp = (IComplexProperty)p;
                    int index = p.Ordinal;
                    
                    if (!cmp.RefIdPropName.IsNullOrWhiteSpace())
                    {
                        IDataEntityProperty ref_property = null;
                        if (dt.Properties.TryGetValue(cmp.RefIdPropName, out ref_property))
                            index = ((IDataEntityProperty)ref_property).Ordinal;
                    }
                    ComplexPropertyMap prop = new ComplexPropertyMap(index, cmp,this);
                    propMaps.Add(prop);
                    continue;
                }
                if (p is ICollectionProperty) {
                    CollectionPropertyMap prop = new CollectionPropertyMap(p.Ordinal, (ICollectionProperty)p,this);
                    propMaps.Add(prop);
                }

            }



            return propMaps;
        }


        public string GetExtendName(IDataEntityType dt)
        {
            if (dt is DynamicObjectType) return ((DynamicObjectType)dt).ExtendName;
            return dt.Name;
        }

        public  interface IPropertyMap
        {
            void MapValue(object obj, List<object> values);
        }



        private  class SimplePropertyMap : IPropertyMap
        {
            private int index;
            private IDataEntityProperty prop;
            private Func<object, object> converter;

            public SimplePropertyMap(int ordinal, IDataEntityProperty p)
            {
                this.index = ordinal;
                this.prop = p;
                this.converter = GetConverter(this.prop.PropertyType);
            }

            private Func<object, object> GetConverter(Type propertyType)
            {
                if (propertyType == typeof(long))
                {
                    return GetLongConverter();
                }
                return null;
            }

            private Func<object, object> GetLongConverter()
            {
                return value =>
                {
                    if (value is int intValue)
                    {
                        value = (long)intValue; // 将int值转换为long
                    }
                    else if (value is string stringValue && long.TryParse(stringValue, out long parsedValue))
                    {
                        value = parsedValue; // 从字符串解析出long值
                    }
                    // 可以继续添加其他类型的处理

                    return value;
                };
            }

            public void MapValue(object obj, List<object> values)
            {
                object value = values[index];
                if (value == null)
                    return;
                value = this.converter(value);
                this.prop.SetValueFast(obj, value);
            }
        }
        private class ComplexPropertyMap : IPropertyMap
        {
            private int index;

            private IComplexProperty prop;
            private readonly DataEntitySerializerReader reader;
            public ComplexPropertyMap(int ordinal, IComplexProperty p, DataEntitySerializerReader dataEntitySerializerReader)
            {
                this.index = ordinal;
                this.prop = p;
                this.reader = dataEntitySerializerReader;
            }




            public void MapValue(object obj, List<object> values)
            {
                object id = values[this.index];
                if (id != null)
                {
                    object cmpObj = null;
                    
                    var extendName = reader.GetExtendName(prop.ComplexType);
                    Dictionary<object, object> refObjs = (Dictionary<object, object>)reader.refComplexPropertys.GetOrDefault(extendName);
                    if (refObjs == null)
                    {
                        refObjs = new Dictionary<object, object>();
                        cmpObj = ReadComplexObject(id, extendName);
                        refObjs[id]=cmpObj;
                        this.reader.refComplexPropertys[extendName] = refObjs;
                    }
                    else
                    {
                        object o = refObjs.GetOrDefault(id);
                        if (o == null)
                        {
                            cmpObj = ReadComplexObject(id, extendName);
                            refObjs[id] = cmpObj;
                        }
                        else
                        {
                            cmpObj = o;
                        }
                    }
                    this.prop.SetValue(obj, cmpObj);
                }
            }


            private object ReadComplexObject(object id, string extendName)
            {

                Dictionary<object, List<object>> refObjData = reader.refComplexPropertyData.GetOrDefault(extendName);
                if (refObjData!=null)
                {
                    List<object> refValues = refObjData.GetOrDefault(id);
                    return reader.ReadObject(prop.ComplexType, refValues, reader.GetPropertyMaps(prop.ComplexType));
                }
                return null;
            }


        }

        private class CollectionPropertyMap : IPropertyMap
        {



            private int index;

            private ICollectionProperty prop;
            private readonly DataEntitySerializerReader reader;
            public CollectionPropertyMap(int ordinal, ICollectionProperty p, DataEntitySerializerReader dataEntitySerializerReader)
            {
                this.index = ordinal;
                this.prop = p;
                this.reader=dataEntitySerializerReader;
            }

            public void MapValue(object obj, List<object> values)
            {
                List<List<object>> objs = (List<List<object>>)values[this.index];
                if (objs == null)
                    return;
                List<Object> tobjs = (List<Object>)this.prop.GetValue(obj);
                IDataEntityType dt = this.prop.ItemType;

                foreach (List<object> vs in objs)
                {
                    tobjs.Add(reader.ReadObject(dt,vs, reader.GetPropertyMaps(dt)));
                }
            }
        }
    }
}
