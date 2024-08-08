using Antlr4.Runtime.Misc;
using WYF.Bos.fulltext;
using WYF.Bos.Orm.impl;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Orm.query.fulltext
{
    public class QMatches
    {
        public  MethodInfo IsDBFulltextM { get; private set; }
        public  Type BaseDataServiceHelperC { get; set; }
        private  class Match
        {
            public string[] values;

            public string[] properties;

            public Match(string[] values, string[] properties)
            {
                this.values = values;
                this.properties = properties;
            }
        }
        public static QParameter ToQParameter(QContext ctx, string rootObjName, Object matchFilterPropertyExpressInfo, QFilter matchFilter)
        {
            Match m;
            bool isMatchSolt = IsMatchSolt(matchFilter.Property);
            if (matchFilterPropertyExpressInfo == null && !isMatchSolt)
                throw new Exception("ftlike的属性不正确。");
            rootObjName = rootObjName.ToLower();
            if (isMatchSolt)
            {
                m = Decode(matchFilter.Value.ToNullString());
            }
            else
            {
                m = new Match(new string[] { matchFilter.Value.ToNullString() }, new string[] { matchFilter.Property });
            }
            QFullTextQuery db_fq = QFullTextQuery.Db(ctx.EntityTypeCache);
            Dictionary<string, HashSet<string>> objectPropertyMap = new Dictionary<string, HashSet<string>>();
            foreach (string p in m.properties)
            {
                String fullObjectName, property;
                int pos = p.LastIndexOf('.');
                if (pos == -1)
                {
                    fullObjectName = rootObjName;
                    property = p;
                }
                else
                {
                    fullObjectName = rootObjName + '.' + p.Substring(0, pos).ToLower();
                    property = p.Substring(pos + 1);
                }
                HashSet<string> ps = objectPropertyMap.GetValueOrDefault(fullObjectName, null);
                if (ps == null)
                {
                    ps = new HashSet<string>();
                    objectPropertyMap[fullObjectName] = ps;
                }
                ps.Add(property.ToLower());
            }
            bool isMultiValue = IsMultiValueMatchSolt(matchFilter.Property);
            QFilter ret = null;

            foreach (var entry in objectPropertyMap)
            {
                //QFilter f = CreateQFilter(ctx, rootObjName, entry.Key, entry.Value, m.values, db_fq, isMultiValue);
                //ret = (ret == null) ? f : ret.Or(f);
            }


            if (ret == null)
                throw new ArgumentException("QFilter cannot be null: " + matchFilter);

            QParameter qp = ret.ToQParameter(ctx);

            qp.MatchTransferQFilter = ret;
            return qp;
        }


        public static bool IsMultiPropertiesMatch(QFilter matchFilter)
        {
            if (IsFtlike(matchFilter) && IsMatchSolt(matchFilter.Property))
            {
                Match m = Decode(matchFilter.Value.ToNullString());
                return (m.properties != null && m.properties.Length > 1);
            }
            return false;
        }
        public static bool IsFtlike(QFilter matchFilter)
        {
            
            return (!matchFilter.IsExpressValue && "ftlike".Equals(matchFilter.Cp.ToLower()));
        }

        public static Object[] QueryPropertyValueByPKs(string entityName, string propertyName, Object[] pks, QContext ctx)
        {
            return QFullTextQuery.Db(ctx.EntityTypeCache).QueryPropertyValueByPKs(entityName, propertyName, pks);
        }

        private  QFilter CreateQFilter(QContext ctx, string rootObjName, string fullObjectName, HashSet<string> properties, string[] values, QFullTextQuery dbq, bool isMultiValue)
        {
            String idProperty;
            bool haveDbMatchProperty = false;
            foreach (string property in properties)
            {
                if (IsDBFulltextField(rootObjName, property))
                {
                    haveDbMatchProperty = true;
                    break;
                }
            }
            string entityName = GetEntityName(ctx, fullObjectName);
            bool fullTextOrmEnable = QFullTextQuery.IsFullTextEnable();
            bool fullTextCustSyncEnable = FullTextFactory.GetFullTextCustSyncQuery().IsConfigFullText(entityName);
            //bool dtsQueryEnable = DataSyncAgent.IsQueryEnable(entityName);
            bool dtsQueryEnable = false;
            bool isESSearch = false;
            if (!isESSearch && !fullTextCustSyncEnable && !haveDbMatchProperty && rootObjName.Equals(fullObjectName))
                return CreateOriginFilter(rootObjName, fullObjectName, properties, values, isMultiValue);
            return null;

        }


        private static QFilter CreateOriginFilter(string rootObjName, string fullObjectName, HashSet<string> properties, string[] values, bool isMultiValue)
        {
            QFilter ret = null;
            foreach (string property in properties)
            {
                string cp = "like";
                foreach (string value in values)
                {
                    string objectProperty;
                    if (rootObjName.Equals(fullObjectName))
                    {
                        objectProperty = property;
                    }
                    else
                    {
                        objectProperty = fullObjectName.Substring(rootObjName.Length + 1) + '.' + property;
                    }
                    if (isMultiValue)
                    {
                        string[] andVS = SplitMultiValue(value);
                        QFilter f = null;
                
                        for (int i = 0; i < andVS.Length; i++)
                        {
                            andVS[i] = AppendWildcard(EscapeWildcard(andVS[i]));
                            QFilter ff = new QFilter(objectProperty, cp, andVS[i]);
                            f = (f == null) ? ff : f.And(ff);
                        }


                        ret = (ret == null) ? f : ret.Or(f);
                    }
                    else
                    {
                        //value = AppendWildcard(EscapeWildcard(value));
                        //QFilter f = new QFilter(objectProperty, cp, value);
                        //ret = (ret == null) ? f : ret.Or(f);
                    }
                }
            }
            return ret;
        }

        public static string EscapeWildcard(string value)
        {
            return value.Replace("%", "\\%").Replace("_", "\\_");
 
        }

        public static string AppendWildcard(string value)
        {
            if (value == null) return null;

            bool hasWildKey = false;
            char[] chs = value.ToCharArray();
            for (int i = 0, n = chs.Length; i < n; i++)
            {
                char ch = chs[i];
                if (ch == '%' && (i == 0 || chs[i - 1] != '\\'))
                {
                    hasWildKey = true;
                    break;
                }
            }
            if (!hasWildKey)
                return '%' + value + '%';
            return value;
        }

        public static string[] SplitMultiValue(string value)
        {

            if (string.IsNullOrEmpty(value)) return new String[] { " " };
            char[] delim = { ' ' };
            if (delim.Length > 0)
            {
                List<string> ret = new List<string>();
                string split = new string(delim);
                char[] chs = value.ToCharArray();
                StringBuilder token = new StringBuilder();
                for (int i = 0, n = chs.Length; i < n; i++)
                {
                    char ch = chs[i];
                    if (split.IndexOf(ch) != -1)
                    {
                        if (token.ToString().Trim().Length > 0)
                            ret.Add(token.ToString().Trim());
                        token.Length = 0;
                    }
                    else
                    {
                        token.Append(ch);
                    }
                }
                if (split.IndexOf(chs[chs.Length - 1]) == -1)
                    if (token.ToString().Trim().Length > 0)
                        ret.Add(token.ToString().Trim());
                return ret.ToArray();
            }
            value = value.Trim();
            (new String[1])[0] = " ";
            (new String[1])[0] = value;
            return (value.Length == 0) ? new String[1] : new String[1];
        }


        private static string GetEntityName(QContext ctx, string fullObjectName)
        {
            string entityName = string.Empty;
            string[] segs = fullObjectName.Split('.');
            if (segs.Length == 0)
            {
                entityName = segs[0];
            }
            else
            {
                string aloneFullObjectName = fullObjectName;
                EntityItem ei = ctx.GetEntityItem(aloneFullObjectName);
                if (ORMConfiguration.IsEntryEntityType(ei.EntityType))
                {
                    do
                    {
                        aloneFullObjectName = aloneFullObjectName.Substring(0, aloneFullObjectName.LastIndexOf('.'));
                        ei = ctx.GetEntityItem(aloneFullObjectName);
                    } while (ORMConfiguration.IsEntryEntityType(ei.EntityType));
                    entityName = ei.EntityType.Name + fullObjectName.Substring(aloneFullObjectName.Length);
                }
                else
                {
                    entityName = ei.EntityType.Name;
                }
            }
            return entityName;
        }

        private  bool IsDBFulltextField(string rootObjName, string property)
        {
            try
            {
                
                return (IsDBFulltextM.Invoke(BaseDataServiceHelperC, new Object[] { rootObjName, property })).ToBool();
            }
            catch (Exception e)
            {
                //logger.error("invoke BaseDataServiceHelper.isDBFulltext(String entityNumber,String propertyNumber) error:", e);
                return false;
            }
        }

        private static bool IsMultiValueMatchSolt(string propertySolt)
        {
            return "2".Equals(propertySolt);
        }
        private static bool IsMatchSolt(string propertySolt)
        {
            return ("1".Equals(propertySolt) || "2"
              .Equals(propertySolt));
        }
        private static Match Decode(string s)
        {
            int p = s.IndexOf('#');
            string[] properties = s.Substring(0, p).Split(',');
            string[] values = s.Substring(p + 1).Split('\b');
            return new Match(values, properties);
        }
    }
}
