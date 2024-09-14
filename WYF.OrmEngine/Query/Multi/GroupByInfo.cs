using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.OrmEngine.Query.OQL.G.expr;

namespace WYF.OrmEngine.Query.Multi
{
    public class GroupByInfo
    {
        public string FullObjectName { get; private set; }

        public PropertySegExpress PropertySegExpress { get; private set; }
        public static List<GroupByInfo> GetGroupByList(string groupBys, string rootObjName)
        {
            if (string.IsNullOrEmpty(groupBys))
                return new List<GroupByInfo>();
            return GroupBys.ParseFrom(groupBys).CreateGroupByInfos(rootObjName);
        }

        public GroupByInfo(string fullObjectName, PropertySegExpress propertySegExpress)
        {
            this.FullObjectName = fullObjectName;
            this.PropertySegExpress = propertySegExpress;
        }

        public String ToGroupByString(QContext ctx)
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
            return sb.ToString();
        }

    }
}
