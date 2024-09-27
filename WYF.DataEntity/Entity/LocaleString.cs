using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    [Serializable]
    public class LocaleString : LocaleValue<string>, ILocaleString
    {
        private static readonly long serialVersionUID = 3914160024089663533L;


        public string GetLocaleValue()
        {
            return ToString();
        }

        public LocaleString() { }

        public LocaleString(string localeId, string value) : base(localeId, value) { }

        public LocaleString(string value) : base(LangExtensions.Get().ToString(), value) { }

        public LocaleString(string localeId, object value) : base(localeId, value?.ToString()) { }

 
        public override string ToString()
        {
            string val = this[LangExtensions.Get().ToString()];
            return val ?? this["zh_CN"];
        }

    
        public static LocaleString FromMap(IDictionary<string, string> values)
        {
            var val = new LocaleString();
            foreach (var entry in values)
            {
                val.SetItem(entry.Key, entry.Value);
            }
            return val;
        }
    }
}
