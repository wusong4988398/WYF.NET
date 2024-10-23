using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 数据实体缓存管理器接口
    /// </summary>
    public interface IDataEntityCacheManager
    {
        /// <summary>
        /// 获取数据实体类型
        /// </summary>
        /// <returns>数据实体类型</returns>
        IDataEntityType GetDataEntityType();

        /// <summary>
        /// 从缓存中获取指定对象标识符（OIDs）对应的数据实体
        /// </summary>
        /// <param name="oids">对象标识符数组</param>
        /// <returns>键值对形式的数据实体字典</returns>
        IDictionary<object, object> Get(object[] oids);
    }
}
