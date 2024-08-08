using WYF.Bos.algo;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query.multi;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.optimize
{
    public class QueryOptimizater
    {
        private ORMOptimization opt;
        public bool IsQuerySingleDB { get; set; }
        private QContext allCtx;
        public QueryOptimizater(ORMOptimization opt, QContext allCtx)
        {
            this.opt = opt;
            this.allCtx = allCtx;
        }
        public DataSet Query(string algoKey, SingleQuery[] queries)
        {
            QueryTreeNode root = QueryTreeNode.Create(queries);

            this.IsQuerySingleDB = (queries.Length == 1);
            if (queries.Length == 1)
            {
                //ReplaceOrderBy(root);
                return FinallySingleQuery(algoKey, root);
            }
            return null;
        }

        DataSet FinallySingleQuery(string algoKey, QueryTreeNode root)
        {
            return root.SingleQuery.Query(algoKey, true);
        }
    }
}
