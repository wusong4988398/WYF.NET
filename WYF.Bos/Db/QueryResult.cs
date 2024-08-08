using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public  class QueryResult<T>
    {
        public   T Result { get; private set; }

        public QueryResource Resource { get; private set; }
        public QueryResult(T result, QueryResource resource)
        {
            this.Result = result;
            this.Resource = resource;
        }
    }
}
