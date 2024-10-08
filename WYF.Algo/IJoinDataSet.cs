using System.Data;

namespace WYF.Algo
{
    /// <summary>
    /// IDataSet进行join时生成的一种过渡数据形态,需要使用finish生成新的IDataSet。
    /// </summary>
    public interface IJoinDataSet
    {
        /// <summary>
        /// 进行连接条件设置
        /// </summary>
        /// <param name="leftField">左连接字段</param>
        /// <param name="rightField">右连接字段</param>
        /// <returns></returns>
        IJoinDataSet On(string leftField, string rightField);
        /// <summary>
        /// join后返回需要的字段
        /// </summary>
        /// <param name="leftFields">左连接字段</param>
        /// <param name="rightFields">右连接字段</param>
        /// <returns></returns>
        IJoinDataSet Select(string[] leftFields, string[] rightFields);
        /// <summary>
        /// join后返回需要的字段
        /// </summary>
        /// <param name="fields">返回左表字段</param>
        /// <returns></returns>
        IJoinDataSet Select(params string[] fields);
        /// <summary>
        /// 选择join的方式
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        IJoinDataSet Hint(JoinHint hint);
        /// <summary>
        /// 生成DataSet
        /// </summary>
        /// <returns></returns>
        DataSet Finish();
    }
}