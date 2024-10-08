using System.Data;

namespace WYF.Algo
{
    /// <summary>
    /// 调用DataSet.groupBy字后返回的一个中间状态结果集，可以对这个结果集继续进行聚合处理，
    ///如果要得到最终的结果需要finish。
    /// </summary>
    public interface IGroupbyDataSet
    {
        /// <summary>
        /// 类似于 sum(field)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        IGroupbyDataSet Sum(string field);
        /// <summary>
        /// 类似于sum(expr) as alias
        /// </summary>
        /// <param name="expr">支持计算表达式, + - * ()等</param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IGroupbyDataSet Sum(string expr, string alias);
        /// <summary>
        /// 分组后求平均值
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        IGroupbyDataSet Avg(string field);
        /// <summary>
        /// 分组后求平均值
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IGroupbyDataSet Avg(string expr, string alias);
        /// <summary>
        /// 分组后求最大值
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        IGroupbyDataSet Max(string field);
        /// <summary>
        /// 分组后求最大值
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IGroupbyDataSet Max(string expr, string alias);
        /// <summary>
        /// 分组后求最小值
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        IGroupbyDataSet Min(string field);

        IGroupbyDataSet Min(string expr, string alias);
        /// <summary>
        /// 用max(field)计算最大行，结果取propertyField字段
        /// </summary>
        /// <param name="paramString1"></param>
        /// <param name="paramString2"></param>
        /// <returns></returns>
        IGroupbyDataSet MaxP(string field, string propertyField);
        /// <summary>
        /// 用max(expr)计算最大行，结果取propertyField字段并给其起别名alias
        /// </summary>
        /// <param name="field"></param>
        /// <param name="propertyField"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IGroupbyDataSet MaxP(string field, string propertyField, string alias);
        /// <summary>
        /// 用min(field)计算最大行，结果取propertyField字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="propertyField"></param>
        /// <returns></returns>
        IGroupbyDataSet MinP(string field, string propertyField);
        /// <summary>
        /// 用min(expr)计算最大行，结果取propertyField字段并给其起别名alias
        /// </summary>
        /// <param name="field"></param>
        /// <param name="propertyField"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IGroupbyDataSet MinP(string field, string propertyField, string alias);
        /// <summary>
        /// 类似于 count(1) as count
        /// </summary>
        /// <returns></returns>
        IGroupbyDataSet Count();
        /// <summary>
        /// 类似于 count(1) as alias
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        IGroupbyDataSet Count(string alias);

        //IGroupbyDataSet agg(CustomAggFunction<?> paramCustomAggFunction, string paramString1, string paramString2);

        IDataSet Finish();
    }
}