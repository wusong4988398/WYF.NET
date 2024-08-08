
using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Entity
{
    /// <summary>
    /// 定义了可选的实体接口，在ORM操作时，此接口并不是必须的
    /// </summary>
    public interface IDataEntityBase
    {
        /// <summary>
        /// 返回当前实体的数据类型
        /// </summary>
        /// <returns></returns>
        IDataEntityType DataEntityType { get; }
        /// <summary>
        /// 返回当前实体的主键值
        /// </summary>
        /// <returns></returns>
        Object PkValue { get; }
    }

}
