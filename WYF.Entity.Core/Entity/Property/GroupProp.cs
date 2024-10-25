using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 基础资料分组字段  主要用于存储基础资料的分组信息
    /// </summary>
    public class GroupProp : BasedataProp
    {
        /// <summary>
        /// 分组表名
        /// </summary>
        [SimpleProperty]
        public string GroupTableName { get; set; }
        /// <summary>
        /// 是否显示分组下级
        /// </summary>
        [SimpleProperty(Name = "ShowTreeLower")]
        public bool IsShowTreeLower { get; set; } = true;
        /// <summary>
        /// 是否需要刷新树
        /// </summary>
        public bool IsNeedRefreshTree { get; set; } = true;
    }
}
