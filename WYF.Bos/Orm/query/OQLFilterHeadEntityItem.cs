using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    [Serializable]
    public class OQLFilterHeadEntityItem : OQLFilterItem
    {
        /// <summary>
        /// 主实体过滤条件
        /// </summary>

        public override string EntityKey
        {
            get
            {
                return "FBillHead";
            }
            set
            {
                base.EntityKey = "FBillHead";
            }
        }
    }
}
