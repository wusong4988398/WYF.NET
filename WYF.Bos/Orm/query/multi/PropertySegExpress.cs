using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public class PropertySegExpress
    {
        private List<Seg> segs = new List<Seg>(50);

        private int s_len = 0;

        private int p_len = 0;

        private Dictionary<String, PropertyField> fieldMap = new Dictionary<String, PropertyField>();


        public List<PropertyField> PropertyFields 
        { 
            get
            {
                return this.fieldMap.Values.ToList();
            } 
        }
        public PropertySegExpress AppendString(String s)
        {
            this.s_len += s.Length;
            this.segs.Add(new Seg(s, false));
            return this;
        }

        public PropertySegExpress AppendProperty(String fullPropertyName)
        {
            this.p_len += fullPropertyName.Length;
            this.segs.Add(new Seg(fullPropertyName, true));
            return this;
        }


        public bool IsExpress()
        {
            int size = this.segs.Count();
            if (size == 0)
                return true;
            if (size == 1)
                return !((Seg)this.segs[0]).IsProperty;
            return true;
        }

        public void PutFieldMap(String fullObjPropertyName, PropertyField pf)
        {
            this.fieldMap[fullObjPropertyName.ToLower()] = pf;
        }

        public List<string> GetFullPropertyNames()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(this.p_len);
            foreach (Seg seg in this.segs)
            {
                if (seg.IsProperty)
                {
                    dict[seg.Value.ToLower()]=seg.Value;
                }
            }
       
            return dict.Values.ToList();
        }

        public string ToString(string rootObjName, QContext ctx)
        {
            StringBuilder s = new StringBuilder(this.s_len + this.p_len);
            foreach (Seg seg in this.segs)
            {
                if (seg.IsProperty)
                {
                    string fullPropertyName = rootObjName + "." + seg.Value;
                    PropertyField pf = this.fieldMap.GetValueOrDefault(fullPropertyName.ToLower(),null);
                    if (pf == null)
                        throw new Exception("属性表达式不正确，只能选取一个对象的属性：");
                    s.Append(pf.ToSelectField(false, ctx));
                    continue;
                }
                s.Append(seg.Value);
            }
            return s.ToString();
        }


        private  class Seg
        {
            private bool _isProperty;
            private string _value;

            public bool IsProperty { get => _isProperty; }
            public string Value { get => _value; }

            public Seg(string value, bool isProperty)
            {
                this._value = value;
                this._isProperty = isProperty;
            }
            public string toString()
            {
                return this.Value;
            }
        }
    }
}
