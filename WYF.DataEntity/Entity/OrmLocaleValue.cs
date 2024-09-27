using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.DataEntity.Entity
{
    [Serializable]
    public class OrmLocaleValue : ILocaleString
    {
        private static readonly long serialVersionUID = -9019668015463680553L;

        private LocaleDynamicObjectCollection _col;
        private IDataEntityProperty _inLocalProperty;
        private string glValue = "";


        public string GetLocaleValue()
        {
            return ((ILocaleString)this).GetLocaleValue();
        }

        public OrmLocaleValue(LocaleDynamicObjectCollection col, DynamicSimpleProperty inLocaleProperty)
        {
            _col = col;
            _inLocalProperty = (IDataEntityProperty)inLocaleProperty;
        }

        public string GetItem(string lcid)
        {
            if (lcid == "GLang")
                return glValue;
            return (string)_col.GetValue(_inLocalProperty, lcid);
        }

        public void SetItem(string lcid, string value)
        {
            if (lcid == "GLang")
            {
                glValue = value;

                return;
            }
            _col.SetValue(_inLocalProperty, lcid, value);
        }

        public int Count => _col.Count;

        public bool IsReadOnly => false;

        public void Add(string key, string value)
        {
            SetItem(key, value);
        }

        public bool ContainsKey(string key)
        {
            DynamicObject? obj = null;
            return _col.FindByLocaleId(key, obj);
        }

        public bool Remove(string key)
        {
            string v = GetItem(key);
            _col.ClearValue(_inLocalProperty);
            return v != null;
        }

        public bool TryGetValue(string key, out string value)
        {
            value = GetItem(key);
            return value != null;
        }

        public string this[string key]
        {
            get => GetItem(key);
            set => SetItem(key, value);
        }

        public ICollection<string> Keys => GetMap().Keys;

        public ICollection<string> Values => GetMap().Values;

        public void Add(KeyValuePair<string, string> item)
        {
            SetItem(item.Key, item.Value);
        }

        public void Clear()
        {
            _col.ClearValue(_inLocalProperty);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ContainsKey(item.Key) && this[item.Key] == item.Value;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return GetMap().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IDictionary<string, string> GetMap()
        {
            var map = new Dictionary<string, string>();
            if (_col.Count() > 0)
            {
                var dynamicProperty = _col.DynamicObjectType.GetProperty("LocaleId");
                foreach (var o in _col)
                {
                    map[(string)dynamicProperty.GetValueFast(o)] = (string)_inLocalProperty.GetValueFast(o);
                }
            }
            return map;
        }

        public override string ToString()
        {
            string val = GetDefaultItem();
            return val ?? this["zh_CN"];
        }

        public string GetDefaultItem()
        {
            if (!string.IsNullOrWhiteSpace(this[LangExtensions.Get().ToString()]))
                return this[LangExtensions.Get().ToString()];
            if (!string.IsNullOrWhiteSpace(glValue))
                return glValue;
            return null;
        }
    }
}
