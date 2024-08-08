using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.db;
using WYF.Bos.Orm.impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public class MultiQueryParameter
    {
        public  DBRoute DbRoute { get;private set; }

        public IDataEntityType EntityType { get; private set; }

        public  string SelectFields { get;  set; }

        public bool ShouldSelectPK { get; private set; }

        public QFilter[] JoinFilters { get; private set; }

        public QFilter[] WhereFilters { get; private set; }

        public string GroupBys { get; private set; }

        public QFilter[] Havings { get; private set; }

        public string OrderBys { get; private set; }

        public int Top { get; private set; }

        public int Start { get; private set; }

        public int Limit { get; private set; }

        public Dictionary<string, IDataEntityType> EntityTypeCache { get; private set; }

        public ORMHint OrmHint { get; private set; }

        public ORMOptimization Optimization { get; private set; }

        public IDistinctable Distinctable { get; private set; }

        public MultiQueryParameter(DBRoute dbRoute, IDataEntityType entityType, string selectFields, bool shouldSelectPK, QFilter[] filters, String groupBys, QFilter[] havings, String orderBys, int top, int start, int limit, Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, ORMOptimization optimization, IDistinctable distinctable)
        {
            this.DbRoute = dbRoute;
            this.EntityType = entityType;
            this.SelectFields = selectFields;
            this.ShouldSelectPK = shouldSelectPK;
            this.GroupBys = groupBys;
            this.Havings = havings;
            this.OrderBys = orderBys;
            this.Top = top;
            this.Start = start;
            this.Limit = limit;
            this.EntityTypeCache = entityTypeCache;
            this.OrmHint = ormHint;
            this.Optimization = optimization;
            this.Distinctable = distinctable;
            DetachFilters(filters);
        }


        private void DetachFilters(QFilter[] filters)
        {
            if (filters != null && filters.Length > 0)
            {
                List<QFilter> joinRecombies = new List<QFilter>();
                List<QFilter> whereRecombies = new List<QFilter>();

                foreach (QFilter filter in filters)
                {
                    if (filter!=null)
                    {
                        foreach (QFilter copy in filter.Copy().Recombine())
                        {
                            if (copy.IsJoinFilter())
                            {
                                joinRecombies.Add(copy);

                            }
                            else
                            {
                                whereRecombies.Add(copy);

                            }
                        }
                    }
                }


                this.JoinFilters = joinRecombies.ToArray();
                this.WhereFilters = whereRecombies.ToArray();
            }
        }
    }
}
