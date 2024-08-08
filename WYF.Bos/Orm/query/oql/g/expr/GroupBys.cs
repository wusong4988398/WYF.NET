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
    public class GroupBys : OQLExpr
    {
        public GroupBys(Expr expr) : base(expr)
        {
        }

        public List<GroupByInfo> CreateGroupByInfos(string rootObjName)
        {
            List<GroupByInfo> ret = new List<GroupByInfo>();
            if (this.Expr is ExprList) {
                foreach (var exp in this.Expr.GetChildren())
                {
                    PropertySegExpress pse = GParser.Parse(exp);
                    ret.Add(CreateGroupByInfo(pse, exp, rootObjName));
                }
              
            } else
            {
                ret.Add(CreateGroupByInfo(ToExpress(), this.Expr, rootObjName));
            }
            return ret;
        }

        public static GroupBys ParseFrom(string groupBys)
        {
            return GParser.ParseGroupBys(groupBys);
        }

        private GroupByInfo CreateGroupByInfo(PropertySegExpress pse, Expr exp, string rootObjName)
        {
            List<string> ps = pse.GetFullPropertyNames();
            string fullObjectName = GetPropertyObjName(ps, rootObjName);
            return new GroupByInfo(fullObjectName, pse);
        }

    }
}
