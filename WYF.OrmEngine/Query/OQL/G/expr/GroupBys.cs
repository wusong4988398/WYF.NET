using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree;
using WYF.OrmEngine.Query.Multi;
using WYF.OrmEngine.Query.OQL.G.Parser;

namespace WYF.OrmEngine.Query.OQL.G.expr
{
    public class GroupBys : OQLExpr
    {
        public GroupBys(Expr expr) : base(expr)
        {
        }

        public List<GroupByInfo> CreateGroupByInfos(string rootObjName)
        {
            List<GroupByInfo> ret = new List<GroupByInfo>();
            if (this.expr is ExprList)
            {
                foreach (var exp in this.expr.GetChildren())
                {
                    PropertySegExpress pse = GParser.Parse(exp);
                    ret.Add(CreateGroupByInfo(pse, exp, rootObjName));
                }

            }
            else
            {
                ret.Add(CreateGroupByInfo(ToExpress(), this.expr, rootObjName));
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
