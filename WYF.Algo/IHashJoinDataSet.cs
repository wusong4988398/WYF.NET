using System.Collections;
using System.Data;

namespace WYF.Algo
{
    /// <summary>
    /// IDataSet进行hashjoin时返回一种过渡形态，还可以继续进行Join等操作，使用finish生成IDataSet。
    /// </summary>
    public interface IHashJoinDataSet
    {
        /// <summary>
        /// 进行hashjoin后，继续添加HashTable形成一个新的HashJoinDataSet。
        /// </summary>
        /// <param name="hashTable">要添加的HashTable</param>
        /// <param name="leftJoinKeyField">连接的字段</param>
        /// <param name="hashTableSelectFields">需要返回的字段</param>
        /// <returns></returns>
        IHashJoinDataSet AddHashTable(IHashTable hashTable, string leftJoinKeyField, string[] hashTableSelectFields);
        /// <summary>
        /// 进行hashjoin后，继续添加HashTable形成一个新的HashJoinDataSet。
        /// </summary>
        /// <param name="hashTable">要添加的HashTable</param>
        /// <param name="leftJoinKeyField">连接的字段</param>
        /// <param name="hashTableSelectFields">需要返回的字段</param>
        /// <param name="includeNotExist">类似left join</param>
        /// <returns></returns>
        IHashJoinDataSet AddHashTable(IHashTable hashTable, string leftJoinKeyField, string[] hashTableSelectFields, bool includeNotExist);
        /// <summary>
        /// 返回的字段
        /// </summary>
        /// <param name="paramArrayOfString"></param>
        /// <returns></returns>
        IHashJoinDataSet SelectLeftFields(string[] leftFields);
        /// <summary>
        /// 可以选择join的方式
        /// </summary>
        /// <param name="paramJoinHint"></param>
        /// <returns></returns>
        IHashJoinDataSet Hint(JoinHint hint);
        /// <summary>
        /// 生成dataset
        /// </summary>
        /// <returns></returns>
        DataSet Finish();
    }
}