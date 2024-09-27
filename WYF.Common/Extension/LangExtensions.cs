using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public static class LangExtensions
    {
        private static readonly Dictionary<string, Lang> enumMap = new Dictionary<string, Lang>();
        private static LangHolder holder;

        static LangExtensions()
        {
            foreach (Lang lang in Enum.GetValues(typeof(Lang)))
            {
                enumMap[lang.ToString().ToLower()] = lang;
            }
        }

        public static CultureInfo GetLocale(this Lang lang)
        {
            return new CultureInfo(lang.GetLangTag());
        }

        public static string GetLangTag(this Lang lang)
        {
            return lang switch
            {
                Lang.zh_CN => "zh-CN",
                Lang.zh_TW => "zh-TW",
                Lang.en_US => "en-US",
                Lang.AF => "af",
                Lang.AR => "ar",
                Lang.BG => "bg",
                Lang.CA => "ca",
                Lang.HR => "hr",
                Lang.CS => "cs",
                Lang.DA => "da",
                Lang.NL => "nl",
                Lang.ET => "et",
                Lang.FI => "fi",
                Lang.FR => "fr",
                Lang.DE => "de",
                Lang.EL => "el",
                Lang.HU => "hu",
                Lang.IS => "is",
                Lang.ID => "id",
                Lang.IT => "it",
                Lang.JA => "ja",
                Lang.KO => "ko",
                Lang.LT => "lt",
                Lang.LV => "lv",
                Lang.MS => "ms",
                Lang.NO => "no",
                Lang.PL => "pl",
                Lang.RU => "ru",
                Lang.SR => "sr",
                Lang.SK => "sk",
                Lang.SL => "sl",
                Lang.SV => "sv",
                Lang.TH => "th",
                Lang.TR => "tr",
                Lang.UK => "uk",
                Lang.VI => "vi",
                Lang.PT => "pt",
                Lang.ES => "es",
                _ => throw new ArgumentOutOfRangeException(nameof(lang), lang, null)
            };
        }

        public static Lang From(string lang)
        {
            if (lang != null && enumMap.TryGetValue(lang.ToLower(), out var result))
            {
                return result;
            }
            return DefaultLang();
        }

        public static Lang From(CultureInfo locale)
        {
            foreach (var lang in Enum.GetValues(typeof(Lang)))
            {
                if (((Lang)lang).GetLocale().Equals(locale))
                {
                    return (Lang)lang;
                }
            }
            return DefaultLang();
        }

        public static Lang DefaultLang()
        {
            //var defaultLang = System.Configuration.ConfigurationManager.AppSettings["default_lang"];
            //if (defaultLang != null && enumMap.TryGetValue(defaultLang.ToLower(), out var result))
            //{
            //    return result;
            //}
            return Lang.zh_CN;
        }

        public static void SetHolder(LangHolder newHolder)
        {
            holder = newHolder;
        }

        public static Lang Get()
        {
             Lang lang = (holder == null) ? DefaultLang() : holder.Get();
             return (lang == null) ? DefaultLang() : lang;

     
           // return lang ?? DefaultLang();
        }
    }

    public class LangHolder
    {
        private readonly Func<Lang> _getLang;

        public LangHolder(Func<Lang> getLang)
        {
            _getLang = getLang;
        }

        public Lang Get()
        {
            return _getLang();
        }
    }
}
