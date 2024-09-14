using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query
{
    public class SelectorItemInfo
    {
        protected string _propertyName;
        internal static string CST_Pattern_ConstSqlNString = string.Format(@"{0}N'[\w\W ]*'{0}", CST_Pattern_NullString);
        internal static string CST_Pattern_ConstSqlString = string.Format(@"{0}'[\w\W ]*'{0}", CST_Pattern_NullString);
        internal static string CST_Pattern_DialectString = @"#[\w\W]+#";
        internal static string CST_Pattern_DigitNumber = string.Format(@"{0}[\d.]+{0}", CST_Pattern_NullString);
        internal static string CST_Pattern_KsqlDateTime = string.Format(@"{{ts'[\d]{{4}}-[\d]{{1,2}}-[\d]{{1,2}}{0}[0-9]{{2}}:[0-9]{{2}}:[0-9]{{2}}'}}", CST_Pattern_NullString);
        internal static string CST_Pattern_NormalFieldAliasName = (" as " + CST_Pattern_NormalFieldName);
        internal static string CST_Pattern_NormalFieldName = string.Format(@"{0}[a-zA-Z]\w*{0}", CST_Pattern_NullString);
        internal static string CST_Pattern_NullFieldName = string.Format("{0}null{0}", CST_Pattern_NullString);
        private static string CST_Pattern_NullString = @"[ \t]*";
        internal static string CST_Pattern_SqlFunction = string.Format(@"{0}[a-zA-Z]\w*\([\w,]*\){0}", CST_Pattern_NullString);
        internal static string CST_SelectorKey_ConstPattern = string.Format("(^{0}$)|(^{0}{1}$)|(^{2}$)|(^{2}{1}$)|(^{3}$)|(^{3}{1}$)|(^{4}$)|(^{4}{1}$)|(^{5}$)|(^{5}{1}$)", new object[] { CST_Pattern_NullFieldName, CST_Pattern_NormalFieldAliasName, CST_Pattern_DigitNumber, CST_Pattern_ConstSqlString, CST_Pattern_SqlFunction, CST_Pattern_ConstSqlNString });
        internal static string CST_SelectorKey_DateValueConstPattern = string.Format("(^{0}{1}{0}$)|(^{0}{1}{2}$)", CST_Pattern_NullString, CST_Pattern_KsqlDateTime, CST_Pattern_NormalFieldAliasName);
        internal static string CST_SelectorKey_DefaultPattern = string.Format("(^{0}$)|(^{0}{1}$)", CST_Pattern_NormalFieldName, CST_Pattern_NormalFieldAliasName);
        internal const string CST_SelectorKey_DigitPattern = @"(^[ ]*\d+[ ]*$)|(^[ ]*\d+.\d+[ ]*$)";
        internal static string CST_SelectorKey_ExprFieldPattern = string.Format(@"(N\'[\w-: .]+\')|[\w.]+|(\'[\w-: .]+\')|({0})", CST_Pattern_DialectString);
        internal static string CST_SelectorKey_ExprPattern = string.Format(@"(\+)|(\-)|(\*)|(\/)|({0}case[\s\S]+when{0})", CST_Pattern_NullString);
        internal static string CST_SelectorKey_RefPropPattern = string.Format(@"(^{0}(\.{0}){{1,2}}[ ]*$)|(^{0}(\.{0}){{1,2}}[ ]*{1}$)", CST_Pattern_NormalFieldName, CST_Pattern_NormalFieldAliasName);
        internal static Regex Regex_ConstPattern = new Regex(CST_SelectorKey_ConstPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_DataValueConstPattern = new Regex(CST_SelectorKey_DateValueConstPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_DefaultPattern = new Regex(CST_SelectorKey_DefaultPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_DialectString = new Regex(CST_Pattern_DialectString, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_DigitPattern = new Regex(@"(^[ ]*\d+[ ]*$)|(^[ ]*\d+.\d+[ ]*$)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_ExprFieldPattern = new Regex(CST_SelectorKey_ExprFieldPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_ExprPattern = new Regex(CST_SelectorKey_ExprPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        internal static Regex Regex_RefPropPattern = new Regex(CST_SelectorKey_RefPropPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public SelectorItemInfo(string key)
        {
            if (!this.IsSupport(key))
            {
                throw new NotSupportedException(string.Format("当前查询串无法解析--{0}", key));
            }
            this.Parse(key);
        }
        protected virtual void Parse(string strSelItem)
        {
            string[] strArray = strSelItem.Split(new string[] { " as ", " AS ", " As ", " aS " }, StringSplitOptions.None);
            this.PropertyName = (strArray.Length > 1) ? strArray[1].Trim() : string.Empty;
            this.Key = strArray[0].Trim();
        }
        protected virtual bool IsSupport(string strSelItem)
        {
            return Regex_DefaultPattern.IsMatch(strSelItem);
        }
        public string Key { get; set; }

        public string PropertyName
        {
            get
            {
                if (this._propertyName.IsNullOrEmpty())
                {
                    return this.Key;
                }
                return this._propertyName;
            }
            set
            {
                this._propertyName = value;
            }
        }
    }
}
