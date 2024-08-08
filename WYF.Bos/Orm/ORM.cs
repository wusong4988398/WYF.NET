using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm
{
    public interface ORM
    {
        static ORM Create()
        {
            return (ORM)new ORMImpl();
        }

        DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int top, IDistinctable distinctable);
        DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, IDistinctable distinctable);
        DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, Func<IDataEntityType, Dictionary<string, bool>,bool> distinctable);
    }
}
