using System;
namespace WYF.DataEntity.Entity
{
    /// <summary>
    /// 分录行状态信息
    /// </summary>
    [Serializable]
    public class EntryInfo
    {
        private int rowCount;

        private int startRowIndex;

        private int pageSize = 10000;
        /// <summary>
        /// 分录总行数。如果未设置，返回空(null)
        /// </summary>
        public int RowCount { get => rowCount; set => rowCount = value; }
        /// <summary>
        /// 分录页起始行索引
        /// </summary>
        public int StartRowIndex { get => startRowIndex; set => startRowIndex = value; }
        /// <summary>
        /// 分录的每页条数
        /// </summary>
        public int PageSize { get => pageSize; set => pageSize = value; }

        /// <summary>
        /// 判断当前索引是否在当前分录页
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool InCurrentPage(int index)
        {
            if (index < this.startRowIndex || index > this.startRowIndex + this.pageSize - 1)
                return false;
            return true;
        }
    }
}
