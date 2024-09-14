using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    /// <summary>
    ///一个实体携带的快照对象
    /// </summary>
    [Serializable]
    public class PkSnapshotSet 
    {
    public List<PkSnapshot> Snapshots;

    public PkSnapshotSet():this(0)
    {
       
    }
        /// <summary>
        /// 创建实例并指定快照表的期望大小
        /// </summary>
        /// <param name="capacity"></param>
        public PkSnapshotSet(int capacity)
    {
        this.Snapshots = new List<PkSnapshot>(capacity);
    }
}
}
