using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    /// <summary>
    /// 实体对象查询过滤条件对象
    /// </summary>
    [Serializable]
    public class OQLFilter : Collection<OQLFilterItem>
    {
     
        public void Add(OQLFilterHeadEntityItem item)
        {
            base.Add(item);
        }

        public static OQLFilter CreateHeadEntityFilter(string strFilter)
        {
            OQLFilter filter = new OQLFilter();
            OQLFilterHeadEntityItem item = new OQLFilterHeadEntityItem
            {
                FilterString = strFilter
            };
            filter.Add(item);
            return filter;
        }
    }
}
