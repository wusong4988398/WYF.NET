using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using WYF.OrmEngine.Impl;
using WYF.OrmEngine.Query.Multi;

namespace WYF.OrmEngine.Query.Optimize
{
    public class QueryOptimizater
    {
        public bool QuerySingleDB {  get; set; }
        public ORMOptimization Opt {  get; private set; }

        public QContext AllCtx {  get; private set; }

        public QueryOptimizater(ORMOptimization opt, QContext allCtx)
        {
            this.Opt = opt;
            this.AllCtx = allCtx;
        }



        public IDataSet Query(String algoKey, SingleQuery[] queries)
        {
            QueryTreeNode root = QueryTreeNode.Create(queries);
            this.QuerySingleDB = (queries.Length == 1);
            if (queries.Length == 1)
            {
                //ReplaceOrderBy(root);
                return FinallySingleQuery(algoKey, root);
            }

            return null;

        }
 
        IDataSet FinallySingleQuery(String algoKey, QueryTreeNode root)
        {
            return root.SingleQuery.Query(algoKey, true);
        }


    

    }
}
