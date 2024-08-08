
using Antlr4.Runtime.Misc;

using WYF.Bos.algo.sql.tree;
using WYF.Bos.Orm.query.multi;
using WYF.Bos.Orm.query.oql.g.parser;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.oql.g.expr
{
    public class SelectFields : OQLExpr
    {
        public SelectFields(Expr expr) : base(expr)
        {

        }

        public static SelectFields ParseFrom(string selectFields)
        {
            return GParser.ParseSelectFields(selectFields);
        }

        public List<PropertyField> CreatePropertyFields(string rootObjName)
        {
            List<PropertyField> ret = new List<PropertyField>();
            if (this.Expr is ExprList)
            {
                foreach (Expr exp in this.Expr.GetChildren())
                {
                    PropertySegExpress pse = GParser.Parse(exp);
                    ret.Add(CreatePropertyField(pse, exp, rootObjName));
                }
            }
            else
            {
                ret.Add(CreatePropertyField(ToExpress(), this.Expr, rootObjName));
            }
            


            return ret;
        }


        private PropertyField CreatePropertyField(PropertySegExpress pse, Expr exp, string rootObjName)
        {
            string str1;
            if (exp is UnresolvedStar)
            {
                UnresolvedStar star = (UnresolvedStar)exp;
                return new PropertyField(star.Prefix, "*", null);
            }
            List<string> ps = pse.GetFullPropertyNames();
            string alias = null;
            if (exp is Alias)
            {
                Alias aliasExp = (Alias)exp;
                alias = aliasExp.GetAlias();
                exp = aliasExp.Child;
                if (exp is UnresolvedStar)
                {
                    UnresolvedStar star = (UnresolvedStar)exp;
                    return new PropertyField(star.Prefix, "*", alias);
                }
                str1 = exp.Sql();
                pse = GParser.Parse(exp);
            }
            else
            {
                str1 = alias = exp.Sql();
            }
            string fullObjectName = GetPropertyObjName(ps, rootObjName);
            if (ps.Count > 0)
            {
                if (exp is UnresolvedAttribute)
                {
                    List<string> nameParts = ((UnresolvedAttribute)exp).NameParts;
                    int size = nameParts.Count;
                    if (size > 1)
                    {
                        str1 = nameParts[size - 1];
                        fullObjectName = rootObjName + '.' + string.Join(".", nameParts.GetRange(0, size - 1));
                    }
                }
            }
            else if (exp is Alias)
            {
                int dot = alias.LastIndexOf('.');
                if (dot != -1)
                    fullObjectName = alias.Substring(0, dot).ToLower();
            }
            PropertyField pf = new PropertyField(fullObjectName, str1, alias);
           // pf.SetPropertySegExpress(pse);
            return pf;
        }

        public string GetPropertyObjName(List<string> fullPropertyNames, string rootObjName)
        {
            string fullObjectName = string.Empty;
            if (fullPropertyNames.Count > 0)
            {
                int len = 0;
                foreach (string fullPropertyName in fullPropertyNames)
                {
                    int dot = fullPropertyName.LastIndexOf(".");
                    if (dot != -1)
                    {
                        string sameParentObj = fullPropertyName.Substring(0, dot);
                        if (string.IsNullOrEmpty(fullObjectName) || len > sameParentObj.Length)
                        {
                            fullObjectName = sameParentObj;
                            len = fullObjectName.Length;
                        }
                    }
                }
                if (string.IsNullOrEmpty(fullObjectName))
                {
                    fullObjectName = rootObjName;
                }
                else
                {
                    fullObjectName = rootObjName + "." + fullObjectName;
                }
            }
            else
            {
                fullObjectName = rootObjName;
            }

            return fullObjectName;
        }
    }
}
