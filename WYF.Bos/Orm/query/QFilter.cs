using Antlr4.Runtime.Misc;
using WYF.DataEntity.Metadata;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query.fulltext;
using WYF.Bos.Orm.query.hugein;
using WYF.Bos.Orm.query.multi;
using WYF.Bos.Orm.query.oql.g.expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;


namespace WYF.Bos.Orm.query
{
    [Serializable]
    public class QFilter: QCP
    {
  

        public string Property { get; set; }


        public string Cp { get; set; }

        public  object Value { get; set; }   

        public  bool IsExpressValue { get; set; }

        private List<QFilterNest> nests = new List<QFilterNest>();

        private static readonly String joinSQLEntityPathPrefix = "_JOINSQL_";

        private static readonly String filterOnMetaPropertyEntityPathPrefix = "_FILTERONMETAPROPERTY_";

        private static readonly String joinFilterEntityPathPrefix = "_JOINFILTER_";

        private string joinEntityPath;

        public QFilter() { }
        public QFilter(string property, string cp, Object value):this(property, cp, value, false)
        {
           
        }
        public QFilter(string property, string cp, Object value, bool expressValue)
        {
            __SetProperty(property);
            this.Cp = (string.IsNullOrEmpty(cp)) ? "" : cp.Trim();
            if (!IsCP(this.Cp))
            {
                throw new Exception("条件比较符不正确:" + this.Cp);
            }

            this.Value = value;
            this.IsExpressValue = expressValue;
        }

        public  void __SetProperty(string property)
        {
            this.Property = (property == null) ? "" : property.Trim();
            if (this.Property.Length == 0)
                throw new ArgumentException("条件属性不能为空");
            if (!this.IsExpressValue && this.Property.IndexOf(' ') != -1)
                throw new ArgumentException("条件属性不正确:"+this.Property);
        }
        public void MaskCurrent()
        {
            this.IsExpressValue = false;
            this.Property = "1";
            this.Value = 1;
            this.Cp = "=";
            this.SelfDefinedQParameter = null;

            this.parsedProperty = null;
            this.parsedExpressValue = null;
            this.parsedPropertyFields = null;
        }
        public bool IsCP(string cp)
        {
            int len = cp.Length;
            if (len == 1)
                return (cp.Equals("=") || cp.Equals(">") || cp.Equals("<"));
            if (len == 2)
                return (cp.Equals(">=") || cp.Equals("<=") || cp.Equals("!=") || cp.Equals("<>") || cp.ToLower().Equals("in"));
            if (len == 4)
                return cp.ToLower().Equals("like");
            if (len == 5)
                return cp.ToLower().Equals("match");
            if (len == 6)
                return (cp.ToLower().Equals("not in") || cp.ToLower().Equals("ftlike"));
            if (len == 7)
                return cp.ToLower().Equals("is null");
            if (len == 8)
                return cp.ToLower().Equals("not like");
            if (len == 11)
                return cp.ToLower().Equals("is not null");
            return false;
        }

        public string JoinEntityPath
        {
            get 
            {

                if (this.joinEntityPath == null)
                    return null;
                if (IsJoinSQLFilter())
                    return this.joinEntityPath.Substring("_JOINSQL_".Length + 1);
                if (IsOnMetaJoinPropertyFilter())
                    return this.joinEntityPath.Substring("_FILTERONMETAPROPERTY_".Length + 1);
                if (this.joinEntityPath.StartsWith("_JOINFILTER_"))
                    return this.joinEntityPath.Substring("_JOINFILTER_".Length + 1);
                return this.joinEntityPath;
            }
        }

        private bool innerJoin;

        private bool joinThisEntity;
        //[NonSerialized]
        public QParameter SelfDefinedQParameter { get; set; }
       // [NonSerialized]
        private PropertyField parsedProperty;
        //[NonSerialized]
        private PropertyField parsedExpressValue;
        //[NonSerialized]
        private List<PropertyField> parsedPropertyFields;

        private static readonly String exp_eq = "1=1";

        private static readonly String exp_ne = "1!=1";

        private static readonly String writeObjectVersion = "v1";

        private static readonly String writeObjectHeaderContentPrefix = "=";

        public QFilter Copy()
        {
            return Copy(true);
        }

        public ORMHint.JoinHint GetJoinHint()
        {
            return this.innerJoin ? ORMHint.JoinHint.INNER : ORMHint.JoinHint.LEFT;
        }
        private QFilter Copy(bool withNest)
        {
            QFilter filter = new QFilter();
            filter.Property = this.Property;
            filter.Cp = this.Cp;
            filter.Value = this.Value;
            filter.IsExpressValue = this.IsExpressValue;
            filter.joinEntityPath = this.joinEntityPath;
            filter.innerJoin = this.innerJoin;
            filter.joinThisEntity = this.joinThisEntity;

            if (withNest)
            {
                foreach (QFilterNest nest in GetNests(false))
                {
                    if (nest.IsAnd)
                    {
                        filter = filter.And(nest.filter.Copy());
                        continue;
                    }
                    filter = filter.Or(nest.filter.Copy());
                }
            }

            filter.SelfDefinedQParameter = this.SelfDefinedQParameter;
            filter.parsedPropertyFields = this.parsedPropertyFields;
            filter.parsedProperty = this.parsedProperty;
            filter.parsedExpressValue = this.parsedExpressValue;

            return filter;
        }

        public bool IsJoinSQLFilter()
        {
            return (this.joinEntityPath != null && this.joinEntityPath.StartsWith("_JOINSQL_"));
        }
        public bool IsOnMetaJoinPropertyFilter()
        {
            return (this.joinEntityPath != null && this.joinEntityPath.StartsWith("_FILTERONMETAPROPERTY_"));
        }

        public QFilter[] Recombine()
        {
            List<QFilter[]> list = new List<QFilter[]>(1 + this.nests.Count());
            list.Add(new QFilter[] { Copy(false) });
            for (int i = 0; i < this.nests.Count; i++)
            {
                QFilterNest nest = this.nests[i];
                if (nest.IsAnd)
                {
                    list.Add(nest.filter.Recombine());
                }
                else
                {
                    QFilter before = null;
                    foreach (QFilter[] befores in list)
                    {
                        foreach (QFilter each in befores)
                        {
                            if (before == null)
                            {
                                before = each;
                            }
                            else
                            {
                                before.And(each);
                            }
                        }
                    }
                    list.Clear();
                    if (before != null)
                        list.Add(new QFilter[] { before.Or(nest.filter) });
                }
            }


            if (list.Count == 1)
                return list[0];
            List<QFilter> ret = new List<QFilter>(list.Count + 1);
            foreach (QFilter[] fs in list)
            {
                foreach (var f in fs)
                {
                    ret.Add(f);
                }
            }
            return ret.ToArray();



        }
        public QParameter ToQParameter(QContext ctx)
        {

            string rootObjName, fullObjectName = ctx.EntityItem.FullObjectName;
            int dot = fullObjectName.IndexOf('.');
            if (dot == -1)
            {
                rootObjName = fullObjectName;
            }
            else
            {
                rootObjName = fullObjectName.Substring(0, dot);
            }
            return _ToQParameter(ctx, rootObjName);
        }
        private QParameter _ToQParameter(QContext ctx, string rootObjName)
        {
            this.parsedPropertyFields = new List<PropertyField>();
            PropertyExpressInfo pei = null;
            if (!IsJoinSQLFilter())
                pei = _GetPropertyString(ctx, rootObjName, this.Property);
            string lcp = this.Cp.ToLower();
            List<Object> paramss = new List<Object>();
            StringBuilder s = new StringBuilder(128);
            if (this.SelfDefinedQParameter != null)
            {
                this.SelfDefinedQParameter.Ctx = ctx;
                s.Append(this.SelfDefinedQParameter.GetSql());
                paramss.AddRange((this.SelfDefinedQParameter.Parameters == null) ? new List<Object>() :
                this.SelfDefinedQParameter.Parameters);

            }
            else if (lcp == "ftlike" || lcp == "match")
            {
                QParameter matchQP = (this.SelfDefinedQParameter == null) ? (this.SelfDefinedQParameter = QMatches.ToQParameter(ctx, rootObjName, pei, this)) : this.SelfDefinedQParameter;
                if (QMatches.IsMultiPropertiesMatch(this))
                {
                    s.Append('(').Append(matchQP.GetSql()).Append(')');
                }
                else
                {
                    s.Append(matchQP.GetSql());
                }
                paramss.AddRange(matchQP.Parameters == null ? new List<object>() : matchQP.Parameters);
            }
            else if (this.IsExpressValue)
            {
                s.Append(pei);
                s.Append(' ').Append(this.Cp).Append(' ');
                PropertyField pf = ParsePropertyField(ctx, rootObjName, this.Value.ToNullString(), false);
                if (pf == null)
                {
                    s.Append('#').Append(this.Value);
                }
                else
                {
                    s.Append(pf.ToSelectField(false, ctx));
                }
            }
            else if (lcp.Equals("in") || lcp.Equals("not in"))
            {
                if (this.Value is QParameter)
                {
                    s.Append(pei);
                    s.Append(' ').Append(this.Cp).Append(' ');
                    s.Append('(').Append(((QParameter)this.Value).GetSql()).Append(')');
                    foreach (Object v in ((QParameter)this.Value).Parameters)
                        paramss.Add(v);
                }
                else
                {
                    QParameter inQP = QFilterUtil.GetInQParameter(this.Value);
                    if (inQP != null)
                    {
                        string fp = pei.ToNullString();
                        if ("?".Equals(inQP.GetSql()))
                        {
                            string ecp = lcp.Equals("in") ? "=" : "!=";
                            s.Append(fp).Append(ecp).Append('?');
                        }
                        else if (HugeInConfig.IsEnableOpt() && HugeInConfig.GetOptType() == HugeInConfig.OptType.split_in && (inQP
                          .Parameters).Length > HugeInConfig.InThreshold())
                        {
                            int N = (inQP.Parameters).Length;
                            int SIZE = HugeInConfig.InThreshold();
                            int MOD_VALUE = N % SIZE;
                            int round = N / SIZE + ((MOD_VALUE == 0) ? 0 : 1);
                            s.Append('(');
                            String and_or = lcp.Equals("in") ? " OR " : " AND ";
                            for (int r = 0; r < round; r++)
                            {
                                if (r > 0)
                                    s.Append(and_or);
                                s.Append(fp).Append(' ').Append(this.Cp).Append(' ');
                                int count = SIZE;
                                if (r == round - 1 && MOD_VALUE != 0)
                                    count = MOD_VALUE;
                                s.Append('(').Append(QFilterUtil.MultiParamsSQL(count)).Append(')');
                            }
                            s.Append(')');
                        }
                        else
                        {
                            s.Append(fp).Append(' ').Append(this.Cp).Append(' ');
                            s.Append('(').Append(inQP.GetSql()).Append(')');
                        }
                        foreach (Object value in inQP.Parameters)
                            paramss.Add(value);
                    }
                    else if (lcp.Equals("in"))
                    {
                        s.Append("1!=1");
                    }
                    else
                    {
                        s.Append("1=1");
                    }
                }
            }
            else if (lcp.Equals("is null"))
            {
                s.Append(pei).Append(" is null");
            }
            else if (lcp.Equals("is not null"))
            {
                s.Append(pei).Append(" is not null");
            }
            else if (pei == null)
            {
                //MergeDBBeacon.markAboutHandleAllFilterAndOrderAndGroupBy();
                s.Append("2=2");
            }
            else if (this.Value == QEmptyValue.value)
            {
               
                IDataEntityProperty type = pei.Field.PropertyItem.PropertyType;
                bool isEntry = ORMConfiguration.IsEntryEntityType(pei.Field.EntityType);
                if (isEntry)
                {
                    s.Append(pei);
                    if ("!=".Equals(this.Cp))
                    {
                        s.Append(" is not null");
                    }
                    else
                    {
                        s.Append(" is null");
                    }
                }
                else
                {
                    Type cls;
                    s.Append('(').Append(pei).Append(this.Cp).Append(" ?");
                    if (type is IJoinProperty) {
                        cls = ((IJoinProperty)type).JoinProperty.PropertyType;
                    } else if (type is IComplexProperty) {
                        cls = ((IComplexProperty)type).ComplexType.PrimaryKey.PropertyType;
                    } else if (type.Parent.PrimaryKey == type)
                    {
                        cls = type.PropertyType;
                    }
                    else
                    {
                        throw new Exception(type.Name+ "不是实体主键");
          
                    }
                    if (cls == typeof(int)) {
                    paramss.Add(0);
        } else if (cls == typeof(long)) {
                        paramss.Add(0L);
        } else
{
          paramss.Add(" ");
}
if ("!=".Equals(this.Cp))
{
    s.Append(" AND ").Append(pei).Append(" is not null");
}
else
{
    s.Append(" OR ").Append(pei).Append(" is null");
}
s.Append(')');
      } 
    }

            else if (this.Value is QParameter) {
                s.Append(pei).Append(' ').Append(this.Cp).Append(' ');
                s.Append('(').Append(((QParameter)this.Value).GetSql()).Append(')');
                foreach (Object v in ((QParameter)this.Value).Parameters)
        paramss.Add(v);
            } else
            {
                s.Append(pei).Append(' ').Append(this.Cp).Append(" ?");
      paramss.Add(this.Value);
            }

            foreach (QFilterNest nest in this.nests)
            {
                s.Insert(0, '(');
                s.Append(' ').Append(nest.Op).Append(' ');
                QParameter nestQP = nest.filter._ToQParameter(ctx, rootObjName);
                s.Append(nestQP.GetSql().Trim());
                foreach (Object value in nestQP.Parameters)
        paramss.Add(value);
                s.Append(')');
            }
            if (pei != null)
            {
                IDataEntityProperty jp = pei.Field.PeropertyType;
                if (jp is IJoinProperty) {
                    IDataEntityProperty joinProperty = ((IJoinProperty)jp).JoinProperty;
                    if (joinProperty != joinProperty.Parent.PrimaryKey)
                    {
                        Object[] propertyValues = QMatches.QueryPropertyValueByPKs(joinProperty.Parent.Name, joinProperty
                            .Name, paramss.ToArray(), ctx);
                        paramss.Clear();
                        paramss.AddRange(propertyValues);
               
                    }
                }
            }

            return null;
        }
        private PropertyExpressInfo _GetPropertyString(QContext ctx, String rootObjName, String property)
        {
            PropertyField pf = ParsePropertyField(ctx, rootObjName, property, true);
            if (pf == null)
            {
                //MergeDBBeacon.MarkAboutHandleAllFilterAndOrderAndGroupBy();
                return null;
            }
            return new PropertyExpressInfo(pf.ToSelectField(false, ctx), pf);
        }

        private PropertyField ParsePropertyField(QContext ctx, string rootObjName, string express, bool property)
        {
            PropertyField pf = SelectFields.ParseFrom(express).CreatePropertyFields(rootObjName)[0];
            if (property)
            {
                this.parsedProperty = pf;
            }
            else
            {
                this.parsedExpressValue = pf;
            }
            PropertyField ret = ctx.PutPerformJoinField(pf, IsJoinFilter() ? this : null, property ?
                this.Value.ToNullString() : ((
                IsJoinFilter() && this.joinThisEntity) ? (rootObjName + "." + this.Property) : this.Property));
            if (pf.PropertySegExpress == null)
            {
                ctx.PutPerformJoinField(pf);
                this.parsedPropertyFields.Add(pf);
            }
            else
            {
                foreach (string p in pf.PropertySegExpress.GetFullPropertyNames())
                {
                    PropertyField epf = new PropertyField(rootObjName + '.' + p);
                    ctx.PutPerformJoinField(epf);
                    this.parsedPropertyFields.Add(epf);
                }
            }
            return ret;
        }

        public QFilter Trans(Func<QFilter, QFilter> singleTransFunc)
        {
            foreach (QFilterNest nest in this.nests)
                nest.filter = nest.filter.Trans(singleTransFunc);
            QFilter root = singleTransFunc.Invoke(this);
            List<QFilterNest> oldNests = new List<QFilterNest>(this.nests);
            if (root == this)
                root.nests.Clear();
            foreach (QFilterNest nest in oldNests)
            {
                if (nest.IsAnd)
                {
                    root.And(nest.filter);
                    continue;
                }
                root.Or(nest.filter);
            }
            return root;
        }
        public QFilter And(QFilter p)
        {
            return AddNest(p, "AND");
        }

        public QFilter Or(QFilter p)
        {
            return AddNest(p, "OR");
        }

        public bool IsJoinFilter()
        {
            return (this.joinEntityPath != null && this.joinEntityPath.Length > 0);
        }

    
        private QFilter AddNest(QFilter p, string op)
        {
            if (p != null)
            {
                if (op=="OR"&& p.joinEntityPath!= this.joinEntityPath)
                {
                    throw new ArgumentException("join filter不可与非join filter进行or关联:");
                }
                
                this.nests.Add(new QFilterNest(op, p, this));
            }
            return this;
        }
        public List<QFilterNest> GetNests(bool recursive)
        {
            if (recursive)
            {
                List<QFilterNest> ret = new List<QFilterNest>();
                foreach (QFilterNest nest in this.nests)
                {
                    ret.Add(nest);
                
                    ret.AddRange(nest.Filter.GetNests(true));


                }
           
                return ret;
            }
            return new List<QFilterNest>(this.nests);
        }

        [Serializable]
        public class QFilterNest
        {

            private readonly string and = "AND";

            private readonly string or = "OR";
            public string Op { get; set; }

            public QFilter filter;

            public QFilter parent;

            private QFilterNest() { }

           public QFilterNest(string op, QFilter filter, QFilter parent)
            {
                this.Op = op;
                this.filter = filter;
                this.parent = parent;
            }

         

            public QFilter Filter
            {
                get
                {
                    return this.filter;
                }
            }

            public QFilter Parent
            {
                get
                {
                    return this.parent;
                }
            }

            public void Remove()
            {
                this.parent.nests.Remove(this);
            }

            public void MaskCurrent()
            {
                this.filter.IsExpressValue = false;
                this.filter.Property = "1";
                this.filter.Value = 1;
                this.filter.Cp = IsAnd ? "=" : "!=";
            }

            public override string ToString()
            {
                return this.Op + " " + this.filter;
            }

            public bool IsAnd
            {
                get
                {
                    return "AND" == this.Op;
                }
            }
            
        }

        private  class PropertyExpressInfo
        {
            public string Express { get; set; }

            public PropertyField Field { get; set; }

            public PropertyExpressInfo(string express, PropertyField field)
            {
                this.Express = express;
                this.Field = field;
            }

            public override string ToString()
            {
                return this.Express;
            }
        }
    }


}
