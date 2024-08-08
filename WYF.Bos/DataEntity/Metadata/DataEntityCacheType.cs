using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata
{
    /// <summary>
    /// 实体缓存的类型，可选值包括： 共享型（缺省），隔离型
    /// </summary>
    public enum DataEntityCacheType
    {
        Share=0,
        Multi=1
    }
}
