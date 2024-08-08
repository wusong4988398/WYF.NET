﻿using WYF.Bos.algo;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.db;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query.optimize;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public class MultiQuery
    {
        private readonly SingleQuery[] queries;
        private readonly PropertyField[] selectFields;
        private readonly ORMOptimization optimization;
        private readonly QContext allCtx;
        private bool querySingleDB;
        public static MultiQuery Create(DBRoute dbRoute, IDataEntityType entityType, String selectFields, bool shouldSelectPK, QFilter[] filters, String groupBys, QFilter[] havings, String orderBys, int top, int start, int limit, Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, ORMOptimization optimization, IDistinctable distinctable)
        {
            MultiQueryParameter mp = new MultiQueryParameter(dbRoute, entityType, selectFields, shouldSelectPK, filters, groupBys, havings, orderBys, top, start, limit, entityTypeCache, ormHint, optimization, distinctable);
            return (new MultiQueryBuilder(mp)).Build();
        }
        public MultiQuery(SingleQuery[] queries, PropertyField[] selectFields, ORMOptimization optimization, QContext allCtx)
        {
            this.queries = queries;
            this.selectFields = selectFields;
            this.optimization = optimization;
            this.allCtx = allCtx;
        }
        public DataSet Query(string algoKey)
        {
            QueryOptimizater opt = new QueryOptimizater(this.optimization, this.allCtx);
            DataSet root = opt.Query(algoKey, this.queries);
            this.querySingleDB = opt.IsQuerySingleDB;
            //RowMeta rm = root.getRowMeta();

            return root;
        }
    }
}
