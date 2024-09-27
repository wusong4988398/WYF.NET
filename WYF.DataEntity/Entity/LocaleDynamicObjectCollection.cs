using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.DataEntity.Utils;

namespace WYF.DataEntity.Entity
{

    public delegate bool EqualsDelegate<SourceT, TargetT>(SourceT source, TargetT target);
    public delegate void UpdateDelegate<SourceT, TargetT>(SourceT source, TargetT target);
    public delegate TargetT CreateDelegate<SourceT, TargetT>(SourceT source);
    public delegate void AddDelegate<TargetT>(ICollection<TargetT> list, TargetT target);
    public delegate void RemoveDelegate<TargetT>(ICollection<TargetT> list, TargetT target, int index);
    public class ListSyncFunction<SourceT, TargetT>
    {
        public EqualsDelegate<SourceT, TargetT> EqualsFunc { get; set; }
        public UpdateDelegate<SourceT, TargetT> UpdateFunc { get; set; }
        public CreateDelegate<SourceT, TargetT> CreateFunc { get; set; }
        public AddDelegate<TargetT> AddFunc { get; set; }
        public RemoveDelegate<TargetT> RemoveFunc { get; set; }
    }
    public class LocaleDynamicObjectCollection : DynamicObjectCollection
    {
   

        private IDataEntityProperty _localeIdProperty;
        private const string Key_LocaleId = "LocaleId";

        public LocaleDynamicObjectCollection() { }

        public LocaleDynamicObjectCollection(DynamicObjectType itemDt, object parent)
            : base(itemDt, parent)
        {
            this._localeIdProperty = (IDataEntityProperty)itemDt.GetProperty(Key_LocaleId);
        }

        
        public void Add(int index, DynamicObject item)
        {
            base.Add(index, item);
            
            if (this._localeIdProperty == null)
                this._localeIdProperty =this.DynamicObjectType.Properties[Key_LocaleId];
        }

        public IDataEntityProperty GetLocaleIdProperty()
        {
            return this._localeIdProperty;
        }

        public DynamicObject GetOrCreateItemByLocaleId(string localeId)
        {
            DynamicObject? row = null;
            
            if (FindByLocaleId(localeId, row))
                return row;
            lock (this)
            {
                if (FindByLocaleId(localeId, row))
                    return row;
                row = (DynamicObject)DynamicObjectType.CreateInstance();
                this._localeIdProperty.SetValue(row, localeId);
                Add((DynamicObject)row);
                return row;
            }
        }

        public bool FindByLocaleId(string localeId, DynamicObject? obj)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                obj = this[i];
                if (localeId.Equals(this._localeIdProperty.GetValueFast(obj)))
                    return true;
            }
            obj = null;
            return false;
        }

        public void SetValue(IDataEntityProperty inLocalProperty, string localeId, object newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue as string))
            {
                DynamicObject? obj = null;
                
                if (FindByLocaleId(localeId, obj))
                    inLocalProperty.SetValue(obj, null);
            }
            else
            {
                DynamicObject obj = GetOrCreateItemByLocaleId(localeId);
                inLocalProperty.SetValue(obj, newValue);
            }
        }

        public void SetValue(DynamicProperty inLocalProperty, ILocaleString values)
        {
            var syncFunction = new ListSyncFunction<KeyValuePair<string, string>, DynamicObject>
            {
                
                EqualsFunc = (s, t) => s.Key.Equals(this._localeIdProperty.GetValueFast(t)),
                UpdateFunc = (s, t) => inLocalProperty.SetValue(t, s.Value),
                CreateFunc = s =>
                {
                    if (string.IsNullOrWhiteSpace(s.Value))
                        return null;

                    var row = (DynamicObject)this.DynamicCollectionItemPropertyType.CreateInstance();
                    this._localeIdProperty.SetValue(row, s.Key);
                    return row;
                },
                AddFunc = (col, item) => col.Add(item),
                RemoveFunc = (col, t, index) => inLocalProperty.ResetDTValue(t)
            };

            var entries = new List<KeyValuePair<string, string>>(values.Count);
            foreach (var entry in values)
            {
                entries.Add(entry);
            }

            OrmUtils.Sync(entries, this, syncFunction, true);
            RemoveEmpty();
        }

        public void ClearValue(IDataEntityProperty inLocalProperty)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                DynamicObject obj = this[i];
                inLocalProperty.SetValue(obj, null);
            }
        }

        public bool RemoveIfEmpty(DynamicObject obj)
        {
            if (obj == null)
                throw new ArgumentException("obj");
            DynamicObjectType dt = this.DynamicCollectionItemPropertyType;
            int pkIndex = _localeIdProperty.Ordinal;
            if (dt.PrimaryKey != null)
                pkIndex = dt.PrimaryKey.Ordinal;
            int localeIdIndex = _localeIdProperty.Ordinal;

            foreach (var property in dt.Properties)
            {
                if (property.Ordinal != pkIndex && property.Ordinal != localeIdIndex &&
                    !string.IsNullOrWhiteSpace(property.GetValueFast(obj) as string))
                    return false;
            }

            return Remove(obj);
        }

        public void RemoveEmpty()
        {
            for (int i = Count - 1; i >= 0; i--)
                RemoveIfEmpty(this[i]);
        }

        public object GetValue(IDataEntityProperty inLocalProperty, string localeId)
        {
            DynamicObject? refObj = null;
            if (FindByLocaleId(localeId, refObj))
                return inLocalProperty.GetValueFast(refObj);
            return null;
        }

        public object GetCurrentLocaleValue(IDataEntityProperty inLocalProperty)
        {
            
            string localeId = LangExtensions.Get().ToString();
            return GetValue(inLocalProperty, localeId);
        }

        public void SetCurrentLocaleValue(IDataEntityProperty inLocalProperty, object newValue)
        {
            string localeId = LangExtensions.Get().ToString();
            SetValue(inLocalProperty, localeId, newValue);
        }

        public static object CreateLocaleValue(LocaleDynamicObjectCollection col, DynamicSimpleProperty inLocaleProperty)
        {
            return new OrmLocaleValue(col, inLocaleProperty);
        }
    }
}
