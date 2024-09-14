using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.OrmEngine.Query.OQL.G.expr;

namespace WYF.OrmEngine.Query.Multi
{
    public class OrderByInfo
    {
        public string FullObjectName { get; set; }

        public string Ordering { get; set; }

        public PropertySegExpress PropertySegExpress { get; set; }


        public OrderByInfo(String fullObjectName, PropertySegExpress propertySegExpress, String ordering)
        {
            this.FullObjectName = fullObjectName;
            this.PropertySegExpress = propertySegExpress;
            this.Ordering = (string.IsNullOrEmpty(ordering)) ? "" : ordering.Trim();
        }
        public static List<OrderByInfo> GetOrderByList(string orderBys, string rootObjName)
        {
            if (string.IsNullOrEmpty(orderBys))
                return new List<OrderByInfo>();
            return OrderBys.ParseFrom(orderBys).CreateOrderInfos(rootObjName);
        }


        public string ToOrderByString(QContext ctx)
        {
            String rootObjName;
            StringBuilder sb = new StringBuilder();
            int dot = this.FullObjectName.IndexOf('.');
            if (dot == -1)
            {
                rootObjName = this.FullObjectName;
            }
            else
            {
                rootObjName = this.FullObjectName.Substring(0, dot);
            }
            sb.Append(this.PropertySegExpress.ToString(rootObjName, ctx));
            if (this.Ordering != null && this.Ordering.Length > 0)
                sb.Append(' ').Append(this.Ordering);
            return sb.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.PropertySegExpress.ToString());
            if (!string.IsNullOrEmpty(this.Ordering))
                sb.Append(' ').Append(this.Ordering);
            return sb.ToString();
        }
    }
}
