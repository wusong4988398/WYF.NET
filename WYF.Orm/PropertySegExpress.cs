using System.Collections.Generic;
using System;
using System.Linq;


namespace WYF.SqlParser
{
    public class PropertySegExpress
    {
        private List<Seg> _segs = new List<Seg>(50);
        private int _sLen = 0;
        private int _pLen = 0;
        private Dictionary<string, PropertyField> _fieldMap = new Dictionary<string, PropertyField>();
        //private Literal _literal;

        //public Literal Literal
        //{
        //    get => _literal;
        //    set => _literal = value;
        //}

        public class Seg
        {
            public bool IsProperty { get; }
            public string Value { get; set; }

            public Seg(string value, bool isProperty)
            {
                Value = value;
                IsProperty = isProperty;
            }

            public override string ToString()
            {
                return Value;
            }
        }

        public PropertySegExpress AppendString(string s)
        {
            _sLen += s.Length;
            _segs.Add(new Seg(s, false));
            return this;
        }

        public PropertySegExpress AppendProperty(string fullPropertyName)
        {
            _pLen += fullPropertyName.Length;
            _segs.Add(new Seg(fullPropertyName, true));
            return this;
        }

        public List<string> GetFullPropertyNames()
        {
            var map = _segs.Where(seg => seg.IsProperty)
                           .ToDictionary(seg => seg.Value.ToLower(), seg => seg.Value);
            return new List<string>(map.Values);
        }

        public List<string> GetFullStringNames()
        {
            return _segs.Where(seg => !seg.IsProperty)
                        .Select(seg => seg.Value)
                        .ToList();
        }

        public void PutFieldMap(string fullObjPropertyName, PropertyField pf)
        {
            _fieldMap[fullObjPropertyName.ToLower()] = pf;
        }

        public PropertyField GetFieldMap(string fullObjPropertyName)
        {
            _fieldMap.TryGetValue(fullObjPropertyName.ToLower(), out PropertyField pf);
            return pf;
        }

        public ICollection<PropertyField> PropertyFields => _fieldMap.Values;

        public PropertyField PropertyField
        {
            get
            {
                if (!IsProperty)
                {
                    throw new InvalidOperationException("表达式不可用.");
                }
                return _fieldMap.Values.First();
            }
        }

        public void ReplaceWhenOneProperty(PropertyField pf)
        {
            _fieldMap.Clear();
            string fullObjPropertyName = pf.FullName;
            string fullPropertyName = pf.ParentFullName;
            PutFieldMap(fullObjPropertyName, pf);
            foreach (var seg in _segs)
            {
                if (seg.IsProperty)
                {
                    seg.Value = fullPropertyName;
                }
            }
        }

        public bool IsEmpty => !_segs.Any();

        public bool IsProperty => !IsExpress;

        public bool IsExpress => _segs.Count > 1 || (_segs.Count == 1 && !_segs[0].IsProperty);

        public string ToSingleTableString(bool alias, Func<string, PropertyField> propertyTrans)
        {
            var s = new System.Text.StringBuilder(_sLen + _pLen);
            foreach (var seg in _segs)
            {
                if (seg.IsProperty)
                {
                    string fullPropertyName = seg.Value;
                    PropertyField pf = propertyTrans(fullPropertyName);
                    if (pf == null)
                    {
                        throw new InvalidOperationException($"属性表达式不正确，只能选取一个对象的属性: {ToString()}");
                    }
                    s.Append(pf.ToSingleTableSelectField(false, propertyTrans));
                }
                else
                {
                    s.Append(seg.Value);
                }
            }
            return s.ToString();
        }

        public string ToString(string rootObjName, QContext ctx)
        {
            return ToString(rootObjName, ctx, false);
        }

        public string ToString(string rootObjName, QContext ctx, bool ignoreGL)
        {
            var s = new System.Text.StringBuilder(_sLen + _pLen);
            foreach (var seg in _segs)
            {
                if (seg.IsProperty)
                {
                    string fullPropertyName = $"{rootObjName}.{seg.Value}";
                    PropertyField pf;
                    if (!_fieldMap.TryGetValue(fullPropertyName.ToLower(), out pf))
                    {
                        throw new InvalidOperationException($"属性表达式不正确，只能选取一个对象的属性: {ToString()}");
                    }
                    s.Append(pf.ToSelectField(false, ctx, ignoreGL));
                }
                else
                {
                    s.Append(seg.Value);
                }
            }
            return s.ToString();
        }

        public override string ToString()
        {
            var s = new System.Text.StringBuilder(_sLen + _pLen);
            foreach (var seg in _segs)
            {
                s.Append(seg.Value);
            }
            return s.ToString();
        }
    }
}