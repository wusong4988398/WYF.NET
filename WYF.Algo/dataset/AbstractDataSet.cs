using IronPython.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.dataset.store;
using WYF.Algo.Utils;
using WYF.Algo.Env;
using Environment = WYF.Algo.Env.Environment;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Community.CsharpSqlite.Sqlite3;
namespace WYF.Algo.dataset
{
    public abstract class AbstractDataSet : ResourceHolder, IDataSet
    {
        protected bool closed = false;
        private InnerRowIterator currentIterator = null;

        protected bool autoClosed = false;
        private List<IRow> rows = new List<IRow>();
        private int currentIndex = -1;
        protected RowMeta rowMeta;
        protected IStore Store {  get; set; }
        protected string Name { get; set; }
        protected Environment Environment {  get; set; }
        public IRow Current => currentIndex >= 0 && currentIndex < rows.Count ? rows[currentIndex] : null;
        public List<AbstractDataSet> Outputs { get; set; } = new List<AbstractDataSet>();
        public List<AbstractDataSet> Inputs { get; set; } = new List<AbstractDataSet>();


        object IEnumerator.Current => Current;



        protected AbstractDataSet(string name,Environment env, IStore store):this(name, env, (List<AbstractDataSet>)null)
        {
            
            this.Store = store;
        }

        protected AbstractDataSet(string name, AbstractDataSet child): this(name, child.Environment, child)
        {
            
        }

        protected AbstractDataSet(string name, Environment environment, AbstractDataSet child): this(name, environment, new List<AbstractDataSet>() { child })
        {
            
        }
        public override string GetName()
        {
            return Name;
        }
        public void AddOutput(AbstractDataSet dataSet)
        {
            this.Outputs.Add(dataSet);
        }
        protected AbstractDataSet(string name, Environment environment, List<AbstractDataSet> inputs)
        {
            CheckThreadContext();
            this.Name = name;
            this.Environment = environment;
            if (inputs != null)
            {
                foreach (AbstractDataSet node in inputs)
                {
                    node.AddOutput(this);
                    this.Inputs.Add(node);
                }
            }
                
            environment.RegisterDataSet(this);
            //AlgoMetrics.dataSetCounter.inc();
        }

        private void CheckThreadContext()
        {
            
        }


        public void CheckClosed()
        {
            if (this.autoClosed)
                throw new AlgoException($"{ToString()} has been auto closed because iterator cursor over.");
            if (this.closed)
                throw new AlgoException($"{ToString()} has been closed.");
        }
        
        public override string ToString()
        {
            string s = this.rowMeta == null ? "" : this.rowMeta.ToString();
            return $"DataSet '{this.Name}',{s}";
        }
        public IDataSet AddBalanceField(string name, string expression)
        {
            throw new NotImplementedException();
        }

        public IDataSet AddField(string expr, string alias)
        {
            throw new NotImplementedException();
        }

        public void AddListener(IListener listener)
        {
            throw new NotImplementedException();
        }

        public IDataSet AddNullField(string name)
        {
            throw new NotImplementedException();
        }

        public IDataSet AddNullField(params string[] names)
        {
            throw new NotImplementedException();
        }

        public ICachedDataSet Cache(CacheHint hint)
        {
            throw new NotImplementedException();
        }

        public ICachedDataSetBuilder CacheBuilder(CacheHint hint)
        {
            throw new NotImplementedException();
        }

        public IDataSet Copy()
        {
            throw new NotImplementedException();
        }

        public int Count(string field, bool distinct)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDataSet ExecuteSql(string sql)
        {
            throw new NotImplementedException();
        }

        public IDataSet ExecuteSql(string sql, SqlHint hint)
        {
            throw new NotImplementedException();
        }

        public IDataSet Filter(string expression)
        {
            throw new NotImplementedException();
        }

        public IDataSet Filter(string expression, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet FullJoin(IDataSet rightDataSet)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet FullJoin(IDataSet rightDataSet, JoinHint hint)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IRow> GetEnumerator()
        {
            this.currentIterator = InnerIterator();
            return this.currentIterator;
            //return rows.GetEnumerator();
        }

        public  InnerRowIterator InnerIterator()
        {
            if (this.currentIterator != null)
            {
                if (this.currentIterator.HasItered())
                    throw new AlgoException($"DataSet has bean iterated, can't iterate twice, detail: {ToString()}");
                return this.currentIterator;
            }
            //if (GetOutputCount() > 1)
            //    EnsureStore();
            if (this.Store != null)
                return InnerRowIterator.Wrapper(this.Store.GetRowIterator());
            if (this.Store == null)
            {
                this.currentIterator = CreateIterator();
        //        if (!(this.currentIterator is AutoCloseIterator))
        //this.currentIterator = new AutoCloseIterator(()->autoClose(), this.currentIterator);
                return this.currentIterator;
            }
            return CreateIterator();
        }

        public RowMeta GetRowMeta()
        {
            if (this.rowMeta == null)
                this.rowMeta = CreateTargetRowMeta();
            return this.rowMeta;
        }

        public IGroupbyDataSet GroupBy()
        {
            throw new NotImplementedException();
        }

        public IGroupbyDataSet GroupBy(params string[] fields)
        {
            throw new NotImplementedException();
        }

        public IGroupbyDataSet GroupBy(string[] fields, bool[] ascending)
        {
            throw new NotImplementedException();
        }

        public IHashJoinDataSet HashJoin(IHashTable hashTable, string joinCondition, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public IHashJoinDataSet HashJoin(IHashTable hashTable, string joinCondition, string[] fields, bool useInnerJoin)
        {
            throw new NotImplementedException();
        }

        public bool HasNext()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet Join(IDataSet rightDataSet)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet Join(IDataSet rightDataSet, JoinHint hint)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet Join(IDataSet rightDataSet, JoinType joinType)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet Join(IDataSet rightDataSet, JoinType joinType, JoinHint hint)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet LeftJoin(IDataSet rightDataSet)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet LeftJoin(IDataSet rightDataSet, JoinHint hint)
        {
            throw new NotImplementedException();
        }

        public IDataSet Limit(int start, int length)
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            currentIndex++;
            return currentIndex < rows.Count;
        }

        public IRow Next()
        {
            throw new NotImplementedException();
        }

        public IDataSet OrderBy(params string[] fields)
        {
            throw new NotImplementedException();
        }

        public void Print(bool detailed)
        {
            throw new NotImplementedException();
        }

        public IDataSet Range(int start, int length)
        {
            throw new NotImplementedException();
        }

        public IDataSet RemoveFields(params string[] fields)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public IJoinDataSet RightJoin(IDataSet rightDataSet)
        {
            throw new NotImplementedException();
        }

        public IJoinDataSet RightJoin(IDataSet rightDataSet, JoinHint hint)
        {
            throw new NotImplementedException();
        }

        public IDataSet Select(params string[] fields)
        {
            throw new NotImplementedException();
        }

        public IDataSet Select(string field)
        {
            throw new NotImplementedException();
        }

        public void SetId(string id)
        {
            throw new NotImplementedException();
        }

        public IHashTable ToHashTable(string keyField)
        {
            throw new NotImplementedException();
        }

        public IDataSet Top(int count)
        {
            throw new NotImplementedException();
        }

        public IDataSet Union(IDataSet other)
        {
            throw new NotImplementedException();
        }

        public IDataSet Union(params IDataSet[] others)
        {
            throw new NotImplementedException();
        }

        public IDataSet Where(string expr)
        {
            throw new NotImplementedException();
        }

        public IDataSet Where(string expr, IDictionary<string, object> param)
        {
            throw new NotImplementedException();
        }
        protected abstract InnerRowIterator CreateIterator();
        protected abstract RowMeta CreateTargetRowMeta();
        public abstract void RealClose();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
