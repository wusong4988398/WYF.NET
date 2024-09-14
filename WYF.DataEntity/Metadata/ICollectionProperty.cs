using WYF.DataEntity.Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{

    /// <summary>
    /// 集合属性
    /// </summary>
    public interface ICollectionProperty : IDataEntityProperty
    {
        /// <summary>
        /// 返回此集合属性中项目的实体类型  例如订单的订单明细属性是个集合属性，那么他的CollectionItemPropertyType将是订单明细对象类型
        /// </summary>
        /// <returns></returns>
        IDataEntityType ItemType { get; }
        /// <summary>
        /// 分录行是否必录
        /// </summary>
        /// <returns></returns>
        bool IsEntryMustInput => true;

        /// <summary>
        /// 获取分录行分页相关信息
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public EntryInfo GetEntryInfo(object dataEntity) { return null; }
        
    }

}
