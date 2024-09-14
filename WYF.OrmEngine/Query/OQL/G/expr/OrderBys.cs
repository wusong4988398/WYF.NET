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
    public class OrderBys : OQLExpr
    {
        public OrderBys(Expr expr) : base(expr)
        {
        }

        public static OrderBys ParseFrom(string orderBys)
        {
            return GParser.ParseOrderBys(orderBys);
        }

        public List<OrderByInfo> CreateOrderInfos(string rootObjName)
        {
            List<OrderByInfo> ret = new List<OrderByInfo>();
            if (this.expr is ExprList)
            {
                foreach (Expr exp in this.expr.GetChildren())
                {
                    PropertySegExpress pse = GParser.Parse(exp);
                    ret.Add(CreateOrderInfo(pse, exp, rootObjName));
                }

            }
            else
            {
                ret.Add(CreateOrderInfo(ToExpress(), this.expr, rootObjName));
            }
            return ret;
        }

        private OrderByInfo CreateOrderInfo(PropertySegExpress pse, Expr exp, string rootObjName)
        {
            PropertySegExpress pfSEG = pse;
            string ordering = null;
            if (exp is Alias)
            {
                Alias aliasExp = (Alias)exp;
                pfSEG = GParser.Parse(aliasExp.GetChild());
                ordering = aliasExp.GetAlias();
            }

            List<string> ps = pfSEG.GetFullPropertyNames();
            string fullObjectName = GetPropertyObjName(ps, rootObjName);
            return new OrderByInfo(fullObjectName, pfSEG, ordering);
        }
    }
}
