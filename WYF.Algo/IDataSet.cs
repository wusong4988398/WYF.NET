using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo
{
    /// <summary>
    /// 结果集  模拟数据库的各种基本操作，如select ,where ,groupby等等。
    /// </summary>
    public interface IDataSet : IDisposable, IEnumerable<IRow>, IEnumerator<IRow>
    {
        IDataSet Where(string expr);
        IDataSet Where(string expr, IDictionary<string, object> param);

        IJoinDataSet LeftJoin(IDataSet rightDataSet);
        IJoinDataSet LeftJoin(IDataSet rightDataSet, JoinHint hint);

        IJoinDataSet RightJoin(IDataSet rightDataSet);
        IJoinDataSet RightJoin(IDataSet rightDataSet, JoinHint hint);

        IJoinDataSet FullJoin(IDataSet rightDataSet);
        IJoinDataSet FullJoin(IDataSet rightDataSet, JoinHint hint);

        IDataSet Limit(int start, int length);

        RowMeta GetRowMeta();

        bool HasNext();
        IRow Next();
        bool IsEmpty();

        IDataSet Select(params string[] fields);
        IDataSet Select(string field);
        /// <summary>
        /// 增加字段
        /// </summary>
        /// <param name="expr">新增的字段表达式</param>
        /// <param name="alias">新增字段的别名</param>
        /// <returns></returns>
        IDataSet AddField(string expr, string alias);
        IDataSet AddNullField(string name);
        IDataSet AddNullField(params string[] names);
        IDataSet AddBalanceField(string name, string expression);
        IDataSet RemoveFields(params string[] fields);

        IDataSet Filter(string expression);
        IDataSet Filter(string expression, IDictionary<string, object> parameters);

        IDataSet OrderBy(params string[] fields);

        IGroupbyDataSet GroupBy();
        IGroupbyDataSet GroupBy(params string[] fields);
        IGroupbyDataSet GroupBy(string[] fields, bool[] ascending);
        IHashTable ToHashTable(string keyField);

        IHashJoinDataSet HashJoin(IHashTable hashTable, string joinCondition, params string[] fields);
        IHashJoinDataSet HashJoin(IHashTable hashTable, string joinCondition, string[] fields, bool useInnerJoin);

        IJoinDataSet Join(IDataSet rightDataSet);
        IJoinDataSet Join(IDataSet rightDataSet, JoinHint hint);
        IJoinDataSet Join(IDataSet rightDataSet, JoinType joinType);
        IJoinDataSet Join(IDataSet rightDataSet, JoinType joinType, JoinHint hint);

        IDataSet Union(IDataSet other);
        IDataSet Union(params IDataSet[] others);
        IDataSet Top(int count);
        IDataSet Range(int start, int length);

        IDataSet Copy();

        int Count(string field, bool distinct);

        IDataSet ExecuteSql(string sql);
        IDataSet ExecuteSql(string sql, SqlHint hint);

        ICachedDataSet Cache(CacheHint hint);
        ICachedDataSetBuilder CacheBuilder(CacheHint hint);

        void Print(bool detailed);
        void SetId(string id);
        void AddListener(IListener listener);




    }


    public  interface IListener
    {
     void BeforeClosed() { }
    
     void AfterClosed() { }
    }
}

