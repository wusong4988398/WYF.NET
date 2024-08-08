using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WYF.Bos.DataEntity.Entity
{
    /// <summary>
    /// 单个表对象的快照对象
    /// </summary>
    [Serializable]
    public class PkSnapshot
    {
        /// <summary>
        /// 快照对象对应的表
        /// </summary>
        public String TableName;
        /// <summary>
        /// 快照中所有的主键数据
        /// </summary>
        public Object[] Oids;
        /// <summary>
        /// 每个oid对应父分录的Id，索引号和oids一一对应，只有在子分录中和子分录分页加载时才加载，解决子分录分页删除时分录行号处理需要
        /// </summary>
        public Object[] Opids;
    }
}
