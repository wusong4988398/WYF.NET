using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public interface ILocaleString : ILocaleValue<string>
    {
  

        string GetLocaleValue()
        {
            return this[LangExtensions.Get().ToString()];
        }

        string GetLocaleValue_zh_CN()
        {
            return this[Lang.zh_CN.ToString()];
        }

        string GetLocaleValue_zh_TW()
        {
            return this[Lang.zh_TW.ToString()];
        }

        string GetLocaleValue_en()
        {
            return this[Lang.en_US.ToString()];
        }

        void SetLocaleValue(string value)
        {
            
            SetItem(LangExtensions.Get().ToString(), value);
        }

        void SetLocaleValue_zh_CN(string value)
        {
            SetItem(Lang.zh_CN.ToString(), value);
        }

        void SetLocaleValue_zh_TW(string value)
        {
            SetItem(Lang.zh_TW.ToString(), value);
        }

        void SetLocaleValue_en(string value)
        {
            SetItem(Lang.en_US.ToString(), value);
        }
    }
}
