using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public class ORMCostStat
    {
        private static  ThreadLocal<ORMCostStat> tlCache = new ThreadLocal<ORMCostStat> { Value = new ORMCostStat() };

        private long trace_sql_meta;

        private long sql_meta;

        private long meta;

        private long per_trace_sql_meta;

        private long per_sql_meta;

        private long per_meta;

        private long count_trace_sql_meta;

        private long count_sql_meta;

        private long count_meta;

        public static ORMCostStat Get()
        {
            return tlCache.Value;
        }
        public ORMCostStat BeginTrace()
        {
            this.count_trace_sql_meta++;
            this.per_trace_sql_meta = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return this;
        }

    }
}
