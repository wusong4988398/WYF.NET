
using System.Collections.Generic;

namespace WYF.Algo
{
    public interface ICachedDataSet
    {
        // 获取缓存ID
        string GetCacheId();

        // 获取行数
        int GetRowCount();

        // 获取行元数据
        RowMeta GetRowMeta();

        // 获取指定范围内的行列表
        List<IRow> GetList(int start, int end);

        // 获取指定范围内的行迭代器
        IEnumerator<IRow> GetEnumerator(int start, int end);

        // 关闭数据集
        void Close();
    }

    // 定义构建器接口
    public interface ICachedDataSetBuilder
    {
        // 添加一行
        void Append(IRow row);

        // 添加多行
        void Append(IEnumerable<IRow> rows);

        // 构建CachedDataSet实例
        ICachedDataSet Build();
    }
}