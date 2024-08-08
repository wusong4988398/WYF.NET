using WYF.Bos.algo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public  class DBExt
    {
        public static DataSet queryDataSet(String algoKey, DBRoute dbRoute, String sql, Object[] paramter, QueryMeta queryMeta)
        {
            return DB.QueryDataSet(algoKey, dbRoute, sql, paramter, queryMeta);
        }

    }
}
